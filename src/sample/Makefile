
EXTRA_DIST =  Makefile

# Warning: This is an automatically generated file, do not edit!

srcdir=.
top_srcdir=../..

include $(top_srcdir)/config.make

ifeq ($(CONFIG),DEBUG)
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize- -debug "-define:DEBUG;TRACE"
ASSEMBLY = bin/Debug/SampleApp.exe
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = exe
PROJECT_REFERENCES =  \
	../libcmdline/bin/Debug/CommandLine.dll
BUILD_DIR = bin/Debug/

COMMANDLINE_DLL_SOURCE=../libcmdline/bin/Debug/CommandLine.dll
NUNIT_FRAMEWORK_DLL_SOURCE=../../lib/nunit.framework.dll
SAMPLEAPP_EXE_MDB_SOURCE=bin/Debug/SampleApp.exe.mdb
SAMPLEAPP_EXE_MDB=$(BUILD_DIR)/SampleApp.exe.mdb
COMMANDLINE_DLL_MDB=

endif

ifeq ($(CONFIG),DEBUGTESTS)
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize- -debug "-define:TRACE;DEBUG;UNIT_TESTS"
ASSEMBLY = bin/DebugTests/SampleApp.exe
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = exe
PROJECT_REFERENCES =  \
	../libcmdline/bin/DebugTests/CommandLine.dll
BUILD_DIR = bin/DebugTests/

COMMANDLINE_DLL_SOURCE=../libcmdline/bin/DebugTests/CommandLine.dll
NUNIT_FRAMEWORK_DLL_SOURCE=../../lib/nunit.framework.dll
SAMPLEAPP_EXE_MDB_SOURCE=bin/DebugTests/SampleApp.exe.mdb
SAMPLEAPP_EXE_MDB=$(BUILD_DIR)/SampleApp.exe.mdb
COMMANDLINE_DLL_MDB_SOURCE=../libcmdline/bin/DebugTests/CommandLine.dll.mdb
COMMANDLINE_DLL_MDB=$(BUILD_DIR)/CommandLine.dll.mdb

endif

ifeq ($(CONFIG),RELEASE)
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize+ "-define:TRACE"
ASSEMBLY = bin/Release/SampleApp.exe
ASSEMBLY_MDB = 
COMPILE_TARGET = exe
PROJECT_REFERENCES =  \
	../libcmdline/bin/Release/CommandLine.dll
BUILD_DIR = bin/Release/

COMMANDLINE_DLL_SOURCE=../libcmdline/bin/Release/CommandLine.dll
NUNIT_FRAMEWORK_DLL_SOURCE=../../lib/nunit.framework.dll
SAMPLEAPP_EXE_MDB=
COMMANDLINE_DLL_MDB=

endif

AL=al2
SATELLITE_ASSEMBLY_NAME=$(notdir $(basename $(ASSEMBLY))).resources.dll

PROGRAMFILES = \
	$(COMMANDLINE_DLL) \
	$(NUNIT_FRAMEWORK_DLL) \
	$(SAMPLEAPP_EXE_MDB) \
	$(COMMANDLINE_DLL_MDB)  

BINARIES = \
	$(SAMPLEAPP)  


RESGEN=resgen2

COMMANDLINE_DLL = $(BUILD_DIR)/CommandLine.dll
NUNIT_FRAMEWORK_DLL = $(BUILD_DIR)/nunit.framework.dll
SAMPLEAPP = $(BUILD_DIR)/sampleapp

FILES = \
	Program.cs \
	Properties/AssemblyInfo.cs \
	Properties/ThisAssembly.cs 

DATA_FILES = 

RESOURCES = 

EXTRAS = \
	sampleapp.in 

REFERENCES =  \
	System

DLL_REFERENCES = 

CLEANFILES = $(PROGRAMFILES) $(BINARIES) 

#Targets
all: $(ASSEMBLY) $(PROGRAMFILES) $(BINARIES)  $(top_srcdir)/config.make

include $(top_srcdir)/Makefile.include
#include $(srcdir)/custom-hooks.make



$(eval $(call emit-deploy-target,COMMANDLINE_DLL))
$(eval $(call emit-deploy-target,NUNIT_FRAMEWORK_DLL))
$(eval $(call emit-deploy-wrapper,SAMPLEAPP,sampleapp,x))
$(eval $(call emit-deploy-target,COMMANDLINE_DLL_MDB))


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
	$(call cp,$(COMMANDLINE_DLL),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call cp,$(NUNIT_FRAMEWORK_DLL),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call cp,$(SAMPLEAPP_EXE_MDB),$(DESTDIR)$(libdir)/$(PACKAGE))
	mkdir -p '$(DESTDIR)$(bindir)'
	$(call cp,$(SAMPLEAPP),$(DESTDIR)$(bindir))
	$(call cp,$(COMMANDLINE_DLL_MDB),$(DESTDIR)$(libdir)/$(PACKAGE))
	make post-install-local-hook prefix=$(prefix)

uninstall-local: $(ASSEMBLY) $(ASSEMBLY_MDB)
	make pre-uninstall-local-hook prefix=$(prefix)
	make uninstall-satellite-assemblies prefix=$(prefix)
	$(call rm,$(ASSEMBLY),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(ASSEMBLY_MDB),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(COMMANDLINE_DLL),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(NUNIT_FRAMEWORK_DLL),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(SAMPLEAPP_EXE_MDB),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(SAMPLEAPP),$(DESTDIR)$(bindir))
	$(call rm,$(COMMANDLINE_DLL_MDB),$(DESTDIR)$(libdir)/$(PACKAGE))
	make post-uninstall-local-hook prefix=$(prefix)

install: install-local
uninstall: uninstall-local
clean: clean-local

include $(top_srcdir)/rules.make
