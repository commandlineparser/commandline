
EXTRA_DIST =  Makefile

# Warning: This is an automatically generated file, do not edit!

srcdir=.
top_srcdir=../..

include $(top_srcdir)/config.make

ifeq ($(CONFIG),DEBUG)
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize- -debug "-define:DEBUG;TRACE"
ASSEMBLY = bin/Debug/CommandLine.dll
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = bin/Debug/

NUNIT_FRAMEWORK_DLL_SOURCE=../../lib/nunit.framework.dll
COMMANDLINE_DLL_MDB_SOURCE=bin/Debug/CommandLine.dll.mdb
COMMANDLINE_DLL_MDB=$(BUILD_DIR)/CommandLine.dll.mdb

endif

ifeq ($(CONFIG),DEBUGTESTS)
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize- -debug "-define:TRACE;DEBUG;UNIT_TESTS"
ASSEMBLY = bin/DebugTests/CommandLine.dll
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = bin/DebugTests/

NUNIT_FRAMEWORK_DLL_SOURCE=../../lib/nunit.framework.dll
COMMANDLINE_DLL_MDB_SOURCE=bin/DebugTests/CommandLine.dll.mdb
COMMANDLINE_DLL_MDB=$(BUILD_DIR)/CommandLine.dll.mdb

endif

ifeq ($(CONFIG),RELEASE)
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize+ "-define:TRACE"
ASSEMBLY = bin/Release/CommandLine.dll
ASSEMBLY_MDB = 
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = bin/Release/

NUNIT_FRAMEWORK_DLL_SOURCE=../../lib/nunit.framework.dll
COMMANDLINE_DLL_MDB=

endif

AL=al2
SATELLITE_ASSEMBLY_NAME=$(notdir $(basename $(ASSEMBLY))).resources.dll

PROGRAMFILES = \
	$(NUNIT_FRAMEWORK_DLL) \
	$(COMMANDLINE_DLL_MDB)  

LINUX_PKGCONFIG = \
	$(COMMANDLINE_PC)  


RESGEN=resgen2

NUNIT_FRAMEWORK_DLL = $(BUILD_DIR)/nunit.framework.dll
COMMANDLINE_PC = $(BUILD_DIR)/commandline.pc

FILES = \
	Attributes/BaseOptionAttribute.cs \
	Attributes/HelpOptionAttribute.cs \
	Attributes/OptionListAttribute.cs \
	Attributes/Tests/HelpOptionAttributeFixture.cs \
	Attributes/Tests/ValueListAttributeFixture.cs \
	Parser/CommandLineParser.cs \
	Core/Tests/EnumeratorsFixture.cs \
	Core/Tests/OptionMapFixture.cs \
	Parser/CommandLineOptionsBase.cs \
	Parser/ICommandLineParser.cs \
	Properties/ThisAssembly.cs \
	Parser/Tests/CommandLineParserFixture.cs \
	Tests/Mocks/ComplexOptions.cs \
	Text/CopyrightInfo.cs \
	Core/OptionMap.cs \
	Core/ArgumentParser.cs \
	Core/IArgumentEnumerator.cs \
	Core/LongOptionParser.cs \
	Core/OptionGroupParser.cs \
	Core/OptionInfo.cs \
	Core/PairT.cs \
	Core/ParserState.cs \
	Text/HeadingInfo.cs \
	Text/Tests/CopyrightInfoFixture.cs \
	Text/Tests/HeadingInfoFixture.cs \
	Utility/Tests/ReflectionUtilFixture.cs \
	Utility/Assumes.cs \
	Utility/ReflectionUtil.cs \
	Core/StringArrayEnumerator.cs \
	Attributes/OptionAttribute.cs \
	Properties/AssemblyInfo.cs \
	Core/OneCharStringEnumerator.cs \
	Attributes/ValueListAttribute.cs \
	Core/BadOptionInfo.cs \
	Core/ParsingError.cs \
	Core/PostParsingState.cs \
	Text/BaseSentenceBuilder.cs \
	Text/EnglishSentenceBuilder.cs \
	Utility/StringUtil.cs \
	Parser/CommandLineParserException.cs \
	Tests/Mocks/OptionsBase.cs \
	Tests/DebugStringUtil.cs \
	Attributes/OptionArrayAttribute.cs \
	Attributes/Tests/AttributesFixture.cs \
	Core/TargetWrapper.cs \
	Core/Tests/ArgumentParserFixture.cs \
	Parser/CommandLineParserSettings.cs \
	Parser/Tests/CommandLineParserSettingsFixture.cs \
	Parser/Tests/OptionArrayAttributeParsingFixture.cs \
	Parser/Tests/NullableTypesParsingFixture.cs \
	Parser/Tests/ValueListAttributeParsingFixture.cs \
	Parser/Tests/MutuallyExclusiveParsingFixture.cs \
	Tests/CommandLineParserBaseFixture.cs \
	Tests/BaseFixture.cs \
	Tests/Mocks/BooleanSetOptions.cs \
	Tests/Mocks/SimpleOptionsWithArray.cs \
	Tests/Mocks/SimpleOptionsWithArrayAndValueList.cs \
	Tests/Mocks/SimpleOptionsWithBadOptionArray.cs \
	Tests/Mocks/NullableTypesOptions.cs \
	Tests/Mocks/OptionsWithValueListMaximumZero.cs \
	Tests/Mocks/OptionsWithValueListMaximumThree.cs \
	Tests/Mocks/OptionsWithMultipleSetAndOneOption.cs \
	Tests/Mocks/OptionsWithMultipleSet.cs \
	Tests/Mocks/OptionsWithDefaultSet.cs \
	Tests/Mocks/SimpleOptionsWithHelpOption.cs \
	Tests/Mocks/MixedCaseOptions.cs \
	Tests/Mocks/SimpleOptionsWithEnum.cs \
	Tests/Mocks/SimpleOptionsWithOptionList.cs \
	Tests/Mocks/NumberSetOptions.cs \
	Tests/Mocks/SimpleOptionsWithValueList.cs \
	Tests/Mocks/SimpleOptions.cs \
	Text/FormatOptionHelpTextEventArgs.cs \
	Text/HelpText.cs \
	Text/Tests/HelpTextFixture.cs 

DATA_FILES = 

RESOURCES = 

EXTRAS = \
	Parser \
	commandline.pc.in 

REFERENCES =  \
	System \
	System.Data \
	System.Xml

DLL_REFERENCES =  \
	../../lib/nunit.framework.dll

CLEANFILES = $(PROGRAMFILES) $(LINUX_PKGCONFIG) 

#Targets
all: $(ASSEMBLY) $(PROGRAMFILES) $(LINUX_PKGCONFIG)  $(top_srcdir)/config.make

include $(top_srcdir)/Makefile.include
#include $(srcdir)/custom-hooks.make



$(eval $(call emit-deploy-target,NUNIT_FRAMEWORK_DLL))
$(eval $(call emit-deploy-wrapper,COMMANDLINE_PC,commandline.pc))


$(eval $(call emit_resgen_targets))
$(build_xamlg_list): %.xaml.g.cs: %.xaml
	xamlg '$<'


$(ASSEMBLY_MDB): $(ASSEMBLY)
$(ASSEMBLY): $(build_sources) $(build_resources) $(build_datafiles) $(DLL_REFERENCES) $(PROJECT_REFERENCES) $(build_xamlg_list) $(build_satellite_assembly_list)
	make pre-all-local-hook prefix=$(prefix)
	mkdir -p $(shell dirname $(ASSEMBLY))
	make $(CONFIG)_BeforeBuild
	$(ASSEMBLY_COMPILER_COMMAND) $(ASSEMBLY_COMPILER_FLAGS) -out:$(ASSEMBLY) -target:$(COMPILE_TARGET) $(build_sources_embed) $(build_resources_embed) $(build_references_ref)
	make $(CONFIG)_AfterBuild
	make post-all-local-hook prefix=$(prefix)

install-local: $(ASSEMBLY) $(ASSEMBLY_MDB)
	make pre-install-local-hook prefix=$(prefix)
	make install-satellite-assemblies prefix=$(prefix)
	mkdir -p '$(DESTDIR)$(libdir)/$(PACKAGE)'
	$(call cp,$(ASSEMBLY),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call cp,$(ASSEMBLY_MDB),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call cp,$(NUNIT_FRAMEWORK_DLL),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call cp,$(COMMANDLINE_DLL_MDB),$(DESTDIR)$(libdir)/$(PACKAGE))
	mkdir -p '$(DESTDIR)$(libdir)/pkgconfig'
	$(call cp,$(COMMANDLINE_PC),$(DESTDIR)$(libdir)/pkgconfig)
	make post-install-local-hook prefix=$(prefix)

uninstall-local: $(ASSEMBLY) $(ASSEMBLY_MDB)
	make pre-uninstall-local-hook prefix=$(prefix)
	make uninstall-satellite-assemblies prefix=$(prefix)
	$(call rm,$(ASSEMBLY),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(ASSEMBLY_MDB),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(NUNIT_FRAMEWORK_DLL),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(COMMANDLINE_DLL_MDB),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(COMMANDLINE_PC),$(DESTDIR)$(libdir)/pkgconfig)
	make post-uninstall-local-hook prefix=$(prefix)

install: install-local
uninstall: uninstall-local
clean: clean-local

include $(top_srcdir)/rules.make
