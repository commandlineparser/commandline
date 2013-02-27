PRODUCT = "Command Line Parser Library"
DESCRIPTION = "The Command Line Parser Library offers to CLR applications a clean and concise API for manipulating command line arguments and related tasks."
VERSION = "1.9.71.2"
INF_VERSION = "1.9.71-stable"
AUTHOR = "Giacomo Stelluti Scala"
COPYRIGHT = "Copyright (c) 2005 - 2013 " + AUTHOR
LICENSE_URL = "https://raw.github.com/gsscoder/commandline/master/doc/LICENSE"
PROJECT_URL = "https://github.com/gsscoder/commandline"

require 'albacore'

task :default => [:build, :test]

if RUBY_VERSION =~ /^1\.8/
  class Dir
    class << self
      def exists? (path)
        File.directory?(path)
      end
      alias_method :exist?, :exists?
  end
 end
end

def is_nix
  !RUBY_PLATFORM.match("linux|darwin").nil?
end

def to_win_path(nix_path)
  nix_path.gsub("/", "\\")
end

def invoke_runtime(cmd)
  command = cmd
  if is_nix()
    command = "mono --runtime=v4.0 #{cmd}"
  end
  command
end

CONFIGURATION = "Release"
BUILD_DIR = File.expand_path("build")
OUTPUT_DIR = "#{BUILD_DIR}/out"
SOURCE_DIR = File.expand_path("src")
NUGET_DIR = File.expand_path("nuget")
LIB_DIR = "#{SOURCE_DIR}/libcmdline"
PJ_OUTPUT_DIR ="#{LIB_DIR}/bin/Release"
LIB_ASM = "CommandLine.dll"
LIB_XML = "CommandLine.xml"

msbuild :build_msbuild do |b|
  b.properties :configuration => CONFIGURATION, "OutputPath" => OUTPUT_DIR
  b.targets :Build
  b.solution = "CommandLine.sln"
end

task :build_mdtool do
  mdtool = "mdtool build -c:#{CONFIGURATION} CommandLine.sln"
  sh "#{mdtool}"
  FileUtils.mkdir_p "#{OUTPUT_DIR}"
  FileUtils.cp_r Dir.glob("#{SOURCE_DIR}/tests/bin/#{CONFIGURATION}/*"), "#{OUTPUT_DIR}"
end

#task :build35_mdtool do
#  mdtool = "mdtool build -c:#{CONFIGURATION} src/libcmdline/CommandLine35.csproj"
#  sh "#{mdtool}"
#  FileUtils.mkdir_p "#{OUTPUT_DIR}/NET35"
#  FileUtils.cp_r Dir.glob("#{SOURCE_DIR}/tests/bin/#{CONFIGURATION}/NET35/*"), "#{OUTPUT_DIR}/NET35"
#end

 msbuild :build35_msbuild do |b|
  b.properties :configuration => CONFIGURATION, "OutputPath" => "#{OUTPUT_DIR}/NET35"
  b.targets :Build
  b.solution = "src/libcmdline/CommandLine35.csproj"
end

task :build => :clean do |b|
  build_task = is_nix() ? "build_mdtool" : "build_msbuild"
  Rake::Task[build_task].invoke
end

task :test => :build do
  xunit = invoke_runtime("packages/xunit.runners.1.9.1/tools/xunit.console.clr4.exe")
  sh "#{xunit} #{OUTPUT_DIR}/CommandLine.Tests.dll"
end

assemblyinfo :assemblyinfo do |a|
  a.product_name = PRODUCT
  #a.description = DESCRIPTION
  a.version = a.file_version = VERSION
  a.copyright = COPYRIGHT
  a.custom_attributes :AssemblyInformationalVersion => INF_VERSION, :NeutralResourcesLanguage => "en-US"
  a.output_file = "src/SharedAssemblyInfo.cs"
  a.namespaces "System.Resources"
end

nuspec :nuget_nuspec do |nuspec|
     nuspec.id = "CommandLineParser"
     nuspec.version = INF_VERSION.end_with?("stable") ? VERSION[0..-3] : INF_VERSION
     nuspec.authors = AUTHOR
     nuspec.owners = AUTHOR
     nuspec.description = DESCRIPTION
     nuspec.title = PRODUCT
     nuspec.projectUrl = PROJECT_URL
     nuspec.licenseUrl = LICENSE_URL
     nuspec.requireLicenseAcceptance = "false"
     nuspec.copyright = COPYRIGHT
     nuspec.tags = "command line argument option parser parsing library syntax shell"
     nuspec.iconUrl = "https://github.com/gsscoder/commandline/raw/master/art/CommandLine.png"

     nuspec.file to_win_path("#{PJ_OUTPUT_DIR}/NET35/#{LIB_ASM}"), to_win_path("lib/net35/#{LIB_ASM}")
     nuspec.file to_win_path("#{PJ_OUTPUT_DIR}/NET35/#{LIB_XML}"), to_win_path("lib/net35/#{LIB_XML}")
     nuspec.file to_win_path("#{PJ_OUTPUT_DIR}/#{LIB_ASM}"), to_win_path("lib/net40/#{LIB_ASM}")
     nuspec.file to_win_path("#{PJ_OUTPUT_DIR}/#{LIB_XML}"), to_win_path("lib/net40/#{LIB_XML}")
     nuspec.file to_win_path("#{PJ_OUTPUT_DIR}/#{LIB_ASM}"), to_win_path("lib/net45/#{LIB_ASM}")
     nuspec.file to_win_path("#{PJ_OUTPUT_DIR}/#{LIB_XML}"), to_win_path("lib/net45/#{LIB_XML}")     
     nuspec.file to_win_path("#{NUGET_DIR}/readme.txt"), "readme.txt"

     nuspec.output_file = "#{NUGET_DIR}/CommandLine.nuspec"
end

task :clean do
  FileUtils.rm_rf BUILD_DIR
  FileUtils.rm_rf "src/libcmdline/bin"
  FileUtils.rm_rf "src/libcmdline/obj"
  FileUtils.rm_rf "src/tests/bin"
  FileUtils.rm_rf "src/tests/obj"
  FileUtils.rm_rf "src/demo/bin"
  FileUtils.rm_rf "src/demo/obj"
end
