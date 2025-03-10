<? xml version = "1.0" encoding = "utf-8" ?>
< Project Sdk = "Microsoft.NET.Sdk" >

    < PropertyGroup >

        < ResolveAssemblyWarnOrErrorOnTargetArchitectureMismatch >
            None
        </ ResolveAssemblyWarnOrErrorOnTargetArchitectureMismatch >

        < AppendTargetFrameworkToOutputPath > false </ AppendTargetFrameworkToOutputPath >

        < !--< TargetFramework > net48 </ TargetFramework > -->

        < OutputType > Library </ OutputType >

        < UseWPF > true </ UseWPF >

        < EnableDefaultPageItems > false </ EnableDefaultPageItems >

        < Configurations > Debug_2023; 2023; 2020; 2021; 2022; Debug_2020; Debug_2021; Debug_2022; 2025; Debug_2025; 2024; Debug_2024 </ Configurations >

        < Platforms > AnyCPU; x64 </ Platforms >

    </ PropertyGroup >

    < !--RevitVersion-- >

    < Choose >

        < When Condition = "$(Configuration.Contains('2017'))" >

            < PropertyGroup >

                < RevitVersion > 2017 </ RevitVersion >

                < TargetFramework > net46 </ TargetFramework >

            </ PropertyGroup >

        </ When >

        < When Condition = "$(Configuration.Contains('2018'))" >

            < PropertyGroup >

                < RevitVersion > 2018 </ RevitVersion >

                < TargetFramework > net46 </ TargetFramework >

            </ PropertyGroup >

        </ When >

        < When Condition = "$(Configuration.Contains('2019'))" >

            < PropertyGroup >

                < RevitVersion > 2019 </ RevitVersion >

                < TargetFramework > net47 </ TargetFramework >

            </ PropertyGroup >

        </ When >

        < When Condition = "$(Configuration.Contains('2020'))" >

            < PropertyGroup >

                < RevitVersion > 2020 </ RevitVersion >

                < TargetFramework > net47 </ TargetFramework >

            </ PropertyGroup >

        </ When >

        < When Condition = "$(Configuration.Contains('2021'))" >

            < PropertyGroup >

                < RevitVersion > 2021 </ RevitVersion >

                < TargetFramework > net48 </ TargetFramework >

            </ PropertyGroup >

        </ When >

        < When Condition = "$(Configuration.Contains('2022'))" >

            < PropertyGroup >

                < RevitVersion > 2022 </ RevitVersion >

                < TargetFramework > net48 </ TargetFramework >

            </ PropertyGroup >

        </ When >

        < When Condition = "$(Configuration.Contains('2023'))" >

            < PropertyGroup >

                < RevitVersion > 2023 </ RevitVersion >

                < TargetFramework > net48 </ TargetFramework >

            </ PropertyGroup >

        </ When >

        < When Condition = "$(Configuration.Contains('2024'))" >

            < PropertyGroup >

                < RevitVersion > 2024 </ RevitVersion >

                < TargetFramework > net48 </ TargetFramework >

            </ PropertyGroup >

        </ When >

         < When Condition = "$(Configuration.Contains('2025'))" >

            < PropertyGroup >

                < RevitVersion > 2025 </ RevitVersion >

                < TargetFramework > net8.0 - windows </ TargetFramework >

                < GenerateAssemblyInfo > false </ GenerateAssemblyInfo >

                < UseWPF > true </ UseWPF >

                < UseWindowsForms > true </ UseWindowsForms >

                < ImportWindowsDesktopTargets > true </ ImportWindowsDesktopTargets >

                < EnableDefaultPageItems > false </ EnableDefaultPageItems >

            </ PropertyGroup >

        </ When >

        < Otherwise >

            < PropertyGroup >

                < RevitVersion > 2023 </ RevitVersion >

                < TargetFramework > net48 </ TargetFramework >

            </ PropertyGroup >

        </ Otherwise >

    </ Choose >


    < PropertyGroup >

        < ProjectGuid >{ d35f7794 - b93f - 4f7a - a077 - 2d6332d785d9}</ ProjectGuid >

        < RootNamespace > PCTools </ RootNamespace >

        < AssemblyName > PCTools </ AssemblyName >

    </ PropertyGroup >


    < !--Release-- >

    < PropertyGroup Condition = "!$(Configuration.Contains('Debug'))" >

        < Optimize > true </ Optimize >

        < OutputPath > bin\Release\$(RevitVersion)\$(AssemblyName)\</OutputPath>
		<DefineConstants>REVIT$(RevitVersion)</DefineConstants>
		<NoWarn>MSB3052</NoWarn>
		<DebugType>None</DebugType>
		<PlatformTarget>x64</PlatformTarget>
	</PropertyGroup>

	<!-- Debug -->
	<PropertyGroup Condition="$(Configuration.Contains('Debug'))">
		<DebugSymbols>true</DebugSymbols>
		<OutputPath>bin\Debug\</OutputPath>
		<DefineConstants>DEBUG; TRACE; REVIT$(RevitVersion)</DefineConstants>
		<DebugType>Full</DebugType>
	</PropertyGroup>
	<!-- DebugRevitVersion -->
	<PropertyGroup>
		<DebugRevitVersion>$(RevitVersion)</DebugRevitVersion>
		<StartAction>Program</StartAction>
		<StartProgram>$(ProgramW6432)\Autodesk\Revit $(DebugRevitVersion)\Revit.exe</StartProgram>
	</PropertyGroup>
	<ItemGroup>
	  <Compile Remove="Project1\MVVM\Model\**" />
	  <Compile Remove="Style\Helper\**" />
	  <EmbeddedResource Remove="Project1\MVVM\Model\**" />
	  <EmbeddedResource Remove="Style\Helper\**" />
	  <None Remove="Project1\MVVM\Model\**" />
	  <None Remove="Style\Helper\**" />
	</ItemGroup>
	<ItemGroup>
	  <None Remove="0.Helper\CSHelper\MVVM\Components\View\InputComponentUserControl.xaml" />
	  <None Remove="AutoDim\MVVM\View\AutoDimUserControl.xaml" />
	  <None Remove="AutoDim\MVVM\View\ConfigDimUserControl.xaml" />
	  <None Remove="CloudLocker\View\UI_LockTool.xaml" />
	  <None Remove="Configuation\MVVM\View\ConfiguationUserControl.xaml" />
	  <None Remove="Configuation\MVVM\View\MsgViewUserControl.xaml" />
	  <None Remove="CouplerChecker\MVVM\View\CouplerCheckerUserControl.xaml" />
	  <None Remove="CouplerChecker\MVVM\View\ProgressBarView.xaml" />
	  <None Remove="DimensionRebar\MVVM\View\DimConfig.xaml" />
	  <None Remove="DuplicateAssembly\MVVM\View\DuplicateAssemblyUserControl.xaml" />
	  <None Remove="DuplicateAssembly\MVVM\View\SetMirrorSectionUserControl.xaml" />
	  <None Remove="0.Helper\CSHelper\MVVM\Components\View\StatusComponentUserControl.xaml" />
	  <None Remove="0.Helper\CSHelper\ProgressBar\ProgressBarView.xaml" />
	  <None Remove="HideGridLevel\MVVM\View\HideGridLevelUserControl.xaml" />
	  <None Remove="PCaSchedule\MVVM\View\LoadPresetUserControl.xaml" />
	  <None Remove="PCaSchedule\MVVM\View\PCaScheduleUserControl.xaml" />
	  <None Remove="PCaSchedule\MVVM\View\SavePresetUserControl.xaml" />
	  <None Remove="PCaSchedule\MVVM\View\SelectScheduleUserControl.xaml" />
	  <None Remove="PCBeamRebar\MVVM\View\OpenPreset.xaml" />
	  <None Remove="PCBeamRebar\MVVM\View\PCaBeamRebarUserControl.xaml" />
	  <None Remove="PCBeamRebar\MVVM\View\RebarTop.xaml" />
	  <None Remove="PCBeamRebar\MVVM\View\SavePreset.xaml" />
	  <None Remove="Project1\MVVM\View\Project1UserControl.xaml" />
	  <None Remove="RebarConfig\MVVM\View\RebarConfigUserControl.xaml" />
	  <None Remove="RebarCounter\MVVM\View\RebarCounterUserControl.xaml" />
	  <None Remove="RebarHoisting\MVVM\View\RebarHoistingUserControl.xaml" />
	  <None Remove="RebarOpenning\MVVM\View\RebarOpenningUserControl.xaml" />
	  <None Remove="RebarSectionSetting\MVVM\View\RebarSectionSettingUserControl.xaml" />
	  <None Remove="RebarStirrups\MVVM\View\RebarStirrupsSaveUserControl.xaml" />
	  <None Remove="RebarStirrups\MVVM\View\RebarStirrupsUserControl.xaml" />
	  <None Remove="RebarStirrups\MVVM\View\RebarStirrupTemplatesUserControl.xaml" />
	  <None Remove="Resources\3D.png" />
	  <None Remove="Resources\AboutNew_16x16.ico" />
	  <None Remove="Resources\AboutNew_32x32.ico" />
	  <None Remove="Resources\accessoryoption1.png" />
	  <None Remove="Resources\add-circle.png" />
	  <None Remove="Resources\add_32x32.png" />
	  <None Remove="Resources\AutoDimPca_16x16.ico" />
	  <None Remove="Resources\AutoDimPca_32x32.ico" />
	  <None Remove="Resources\BeamBottom.png" />
	  <None Remove="Resources\BeamMiddle.png" />
	  <None Remove="Resources\BeamRebar_16x16.png" />
	  <None Remove="Resources\BeamRebar_32x32.png" />
	  <None Remove="Resources\BeamTop.png" />
	  <None Remove="Resources\bin.png" />
	  <None Remove="Resources\check-circle.png" />
	  <None Remove="Resources\checkforupdatePCA.json" />
	  <None Remove="Resources\CLOff_16x16.ico" />
	  <None Remove="Resources\CLOff_32x32.ico" />
	  <None Remove="Resources\CLOn_16x16.ico" />
	  <None Remove="Resources\CLOn_32x32.ico" />
	  <None Remove="Resources\ColumnRebar_16x16.png" />
	  <None Remove="Resources\ColumnRebar_32x32.png" />
	  <None Remove="Resources\concreteoption1.png" />
	  <None Remove="Resources\CouplerChecker_16x16.png" />
	  <None Remove="Resources\CouplerChecker_32x32.png" />
	  <None Remove="Resources\Coupler_16x16.png" />
	  <None Remove="Resources\Coupler_32x32.png" />
	  <None Remove="Resources\CreateGrid_16x16.png" />
	  <None Remove="Resources\CreateGrid_32x32.png" />
	  <None Remove="Resources\CUnlock_16x16.ico" />
	  <None Remove="Resources\CUnlock_32x32.ico" />
	  <None Remove="Resources\Dimension_16x16.png" />
	  <None Remove="Resources\Dimension_32x32.png" />
	  <None Remove="Resources\drawschedule16.png" />
	  <None Remove="Resources\DuplicateAssembly_16x16.png" />
	  <None Remove="Resources\DuplicateAssembly_32x32.png" />
	  <None Remove="Resources\duplicate_32x32.png" />
	  <None Remove="Resources\exportexcel16.png" />
	  <None Remove="Resources\gotosheet.png" />
	  <None Remove="Resources\gotosheet_32.png" />
	  <None Remove="Resources\Grid16x16.ico" />
	  <None Remove="Resources\Grid32x32.ico" />
	  <None Remove="Resources\GridOff_16x16.png" />
	  <None Remove="Resources\GridOff_32x32.png" />
	  <None Remove="Resources\GridOn_16x16.png" />
	  <None Remove="Resources\GridOn_32x32.png" />
	  <None Remove="Resources\Grid_XCheck_2022.rfa" />
	  <None Remove="Resources\Grid_XCheck_2023.rfa" />
	  <None Remove="Resources\Grid_XCheck_2024.rfa" />
	  <None Remove="Resources\Grid_XCheck_2025.rfa" />
	  <None Remove="Resources\Grid_YCheck_2022.rfa" />
	  <None Remove="Resources\Grid_YCheck_2023.rfa" />
	  <None Remove="Resources\Grid_YCheck_2024.rfa" />
	  <None Remove="Resources\Grid_YCheck_2025.rfa" />
	  <None Remove="Resources\HookStirrup_16x16.png" />
	  <None Remove="Resources\HookStirrup_32x32.png" />
	  <None Remove="Resources\icon.ico" />
	  <None Remove="Resources\Image_type1.png" />
	  <None Remove="Resources\Image_type10.png" />
	  <None Remove="Resources\Image_type11.png" />
	  <None Remove="Resources\Image_type2.png" />
	  <None Remove="Resources\Image_type3.png" />
	  <None Remove="Resources\Image_type4.png" />
	  <None Remove="Resources\Image_type5.png" />
	  <None Remove="Resources\Image_type6.png" />
	  <None Remove="Resources\Image_type7.png" />
	  <None Remove="Resources\Image_type8.png" />
	  <None Remove="Resources\Image_type9.png" />
	  <None Remove="Resources\Language_16x16.ico" />
	  <None Remove="Resources\Language_32x32.ico" />
	  <None Remove="Resources\LevelCheck_2022.rfa" />
	  <None Remove="Resources\LevelCheck_2023.rfa" />
	  <None Remove="Resources\LevelCheck_2024.rfa" />
	  <None Remove="Resources\LevelCheck_2025.rfa" />
	  <None Remove="Resources\LoadFamily_16x16.png" />
	  <None Remove="Resources\LoadFamily_32x32.png" />
	  <None Remove="Resources\loadpreset16.png" />
	  <None Remove="Resources\Lock_16x16.ico" />
	  <None Remove="Resources\Lock_32x32.ico" />
	  <None Remove="Resources\log-in.png" />
	  <None Remove="Resources\Logo_16x16.png" />
	  <None Remove="Resources\Logo_32x32.png" />
	  <None Remove="Resources\Matbang.png" />
	  <None Remove="Resources\MatCat.png" />
	  <None Remove="Resources\MEI_16x16.png" />
	  <None Remove="Resources\MEI_32x32.png" />
	  <None Remove="Resources\MFUpdate.exe" />
	  <None Remove="Resources\minus-circle.png" />
	  <None Remove="Resources\M_00_2023.rfa" />
	  <None Remove="Resources\M_00_2024.rfa" />
	  <None Remove="Resources\M_00_2025.rfa" />
	  <None Remove="Resources\M_01_2023.rfa" />
	  <None Remove="Resources\M_01_2024.rfa" />
	  <None Remove="Resources\M_01_2025.rfa" />
	  <None Remove="Resources\Ngosen_16x16.png" />
	  <None Remove="Resources\Ngosen_32x32.png" />
	  <None Remove="Resources\Note_1.png" />
	  <None Remove="Resources\Note_2.png" />
	  <None Remove="Resources\Note_3.png" />
	  <None Remove="Resources\Note_4.png" />
	  <None Remove="Resources\Off_16x16.png" />
	  <None Remove="Resources\Off_32x32.png" />
	  <None Remove="Resources\On_16x16.png" />
	  <None Remove="Resources\On_32x32.png" />
	  <None Remove="Resources\Opening_16x16.png" />
	  <None Remove="Resources\Opening_32x32.png" />
	  <None Remove="Resources\PcaSchedule.txt" />
	  <None Remove="Resources\PCaSchedule_16x16.png" />
	  <None Remove="Resources\PCaSchedule_32x32.png" />
	  <None Remove="Resources\PCaSharedParamter.txt" />
	  <None Remove="Resources\PCa_16x16.ico" />
	  <None Remove="Resources\PCa_32x32.ico" />
	  <None Remove="Resources\PCA_Grid.rfa" />
	  <None Remove="Resources\PCA_Grid_2023.rfa" />
	  <None Remove="Resources\PCA_Grid_2024.rfa" />
	  <None Remove="Resources\PCA_Grid_2025.rfa" />
	  <None Remove="Resources\PCA_Grid_Y.rfa" />
	  <None Remove="Resources\PCA_Grid_Y_2023.rfa" />
	  <None Remove="Resources\PCA_Grid_Y_2024.rfa" />
	  <None Remove="Resources\PCA_Grid_Y_2025.rfa" />
	  <None Remove="Resources\PCa_RebarSection_2023.rfa" />
	  <None Remove="Resources\PCa_RebarSection_2024.rfa" />
	  <None Remove="Resources\PCa_RebarSection_2025.rfa" />
	  <None Remove="Resources\PCa_RebarSection_type10_2023.rfa" />
	  <None Remove="Resources\PCa_RebarSection_type10_2024.rfa" />
	  <None Remove="Resources\PCa_RebarSection_type10_2025.rfa" />
	  <None Remove="Resources\PCa_RebarSection_type11_2023.rfa" />
	  <None Remove="Resources\PCa_RebarSection_type11_2024.rfa" />
	  <None Remove="Resources\PCa_RebarSection_type11_2025.rfa" />
	  <None Remove="Resources\PCa_RebarSection_type1_2023.rfa" />
	  <None Remove="Resources\PCa_RebarSection_type1_2024.rfa" />
	  <None Remove="Resources\PCa_RebarSection_type1_2025.rfa" />
	  <None Remove="Resources\PCa_RebarSection_type2_2023.rfa" />
	  <None Remove="Resources\PCa_RebarSection_type2_2024.rfa" />
	  <None Remove="Resources\PCa_RebarSection_type2_2025.rfa" />
	  <None Remove="Resources\PCa_RebarSection_type3_2023.rfa" />
	  <None Remove="Resources\PCa_RebarSection_type3_2024.rfa" />
	  <None Remove="Resources\PCa_RebarSection_type3_2025.rfa" />
	  <None Remove="Resources\PCa_RebarSection_type4_2023.rfa" />
	  <None Remove="Resources\PCa_RebarSection_type4_2024.rfa" />
	  <None Remove="Resources\PCa_RebarSection_type4_2025.rfa" />
	  <None Remove="Resources\PCa_RebarSection_type5_2023.rfa" />
	  <None Remove="Resources\PCa_RebarSection_type5_2024.rfa" />
	  <None Remove="Resources\PCa_RebarSection_type5_2025.rfa" />
	  <None Remove="Resources\PCa_RebarSection_type6_2023.rfa" />
	  <None Remove="Resources\PCa_RebarSection_type6_2024.rfa" />
	  <None Remove="Resources\PCa_RebarSection_type6_2025.rfa" />
	  <None Remove="Resources\PCa_RebarSection_type7_2023.rfa" />
	  <None Remove="Resources\PCa_RebarSection_type7_2024.rfa" />
	  <None Remove="Resources\PCa_RebarSection_type7_2025.rfa" />
	  <None Remove="Resources\PCa_RebarSection_type8_2023.rfa" />
	  <None Remove="Resources\PCa_RebarSection_type8_2024.rfa" />
	  <None Remove="Resources\PCa_RebarSection_type8_2025.rfa" />
	  <None Remove="Resources\PCa_RebarSection_type9_2023.rfa" />
	  <None Remove="Resources\PCa_RebarSection_type9_2024.rfa" />
	  <None Remove="Resources\PCa_RebarSection_type9_2025.rfa" />
	  <None Remove="Resources\Project1_32x32.ico" />
	  <None Remove="Resources\RebarCount_16x16.ico" />
	  <None Remove="Resources\RebarCount_32x32.ico" />
	  <None Remove="Resources\RebarDetail_16x16.png" />
	  <None Remove="Resources\RebarDetail_32x32.png" />
	  <None Remove="Resources\rebaroption1.png" />
	  <None Remove="Resources\rebaroption2.png" />
	  <None Remove="Resources\RebarSection_16x16.ico" />
	  <None Remove="Resources\RebarSection_32x32.ico" />
	  <None Remove="Resources\RebarSetting_16x16.png" />
	  <None Remove="Resources\RebarSetting_32x32.png" />
	  <None Remove="Resources\RebarStirrups_Note_0.png" />
	  <None Remove="Resources\RebarStirrups_Note_1.png" />
	  <None Remove="Resources\RebarStirrups_Note_2.png" />
	  <None Remove="Resources\RebarStirrups_Note_3.png" />
	  <None Remove="Resources\ReinforcingLeft.ico" />
	  <None Remove="Resources\ReinforcingLeft.png" />
	  <None Remove="Resources\ReinforcingRight.ico" />
	  <None Remove="Resources\RemoveRebarSection_16x16.ico" />
	  <None Remove="Resources\RemoveRebarSection_32x32.ico" />
	  <None Remove="Resources\remove_32x32.png" />
	  <None Remove="Resources\savepreset16.png" />
	  <None Remove="Resources\SectionUpdate_16x16.png" />
	  <None Remove="Resources\SectionUpdate_32x32.png" />
	  <None Remove="Resources\select16.png" />
	  <None Remove="Resources\selectschedule16.png" />
	  <None Remove="Resources\SettingTool_16x16.png" />
	  <None Remove="Resources\SettingTool_32x32.png" />
	  <None Remove="Resources\Showhideribbon_16x16.png" />
	  <None Remove="Resources\Showhideribbon_32x32.png" />
	  <None Remove="Resources\StirrupTemplate_2023.rfa" />
	  <None Remove="Resources\StirrupTemplate_2024.rfa" />
	  <None Remove="Resources\Stirrup_16x16.png" />
	  <None Remove="Resources\Stirrup_32x32.png" />
	  <None Remove="Resources\trash-circle.png" />
	  <None Remove="Resources\Unlock_16x16.png" />
	  <None Remove="Resources\Unlock_32x32.png" />
	  <None Remove="Resources\VerfiyModel_16x16.png" />
	  <None Remove="Resources\VerfiyModel_32x32.png" />
	  <None Remove="Resources\Whodidthis_16x16.png" />
	  <None Remove="Resources\Whodidthis_32x32.png" />
	  <None Remove="ShowGridLevel\MVVM\View\ShowGridLevelUserControl.xaml" />
	  <None Remove="ShowHideRibbon\MVVM\View\ShowHideRibbonUserControl.xaml" />
	  <None Remove="Style\Styles.xaml" />
	  <None Remove="Style\Themes.xaml" />
	  <None Remove="VerifyModel\UI\View\ExportModelInformationDlg.xaml" />
	  <None Remove="VerifyModel\UI\View\ModelsCheckDlg.xaml" />
	  <None Remove="VerifyModel\UI\View\ProgressBarView.xaml" />
	  <None Remove="WhoDidThisV1\MVVM\View\WhoDidThisV1UserControl.xaml" />
	  <None Remove="WhoDidThisV2\MVVM\View\WhoDidThisV2UserControl.xaml" />
	</ItemGroup>
	
	<ItemGroup>
		<Reference Include="AdWindows">
			<HintPath>$(ProgramW6432)\Autodesk\Revit $(RevitVersion)\AdWindows.dll</HintPath>
			<Private>False</Private>
		</Reference>
		<Reference Include="DocumentFormat.OpenXml">
		  <HintPath>Libs\DocumentFormat.OpenXml.dll</HintPath>
		</Reference>
		<Reference Include="Microsoft.Windows.Design.Extensibility">
		  <HintPath>Libs\Microsoft.Windows.Design.Extensibility.dll</HintPath>
		</Reference>
		<Reference Include="Microsoft.Xaml.Behaviors">
		  <HintPath>Libs\Microsoft.Xaml.Behaviors.dll</HintPath>
		  <Private>True</Private>
		</Reference>
		<Reference Include="RevitAPI">
			<HintPath>$(ProgramW6432)\Autodesk\Revit $(RevitVersion)\RevitAPI.dll</HintPath>
			<Private>False</Private>
		</Reference>
		<Reference Include="RevitAPIUI">
			<HintPath>$(ProgramW6432)\Autodesk\Revit $(RevitVersion)\RevitAPIUI.dll</HintPath>
			<Private>False</Private>
		</Reference>
		<Reference Include="PresentationCore" Condition="!$(Configuration.Contains('2025'))" />
		<Reference Include="PresentationFramework" Condition="!$(Configuration.Contains('2025'))" />
		<Reference Include="SpreadsheetGear2017.Core">
		  <HintPath>Libs\SpreadsheetGear2017.Core.dll</HintPath>
		</Reference>
		<Reference Include="SpreadsheetGear2017.Design">
		  <HintPath>Libs\SpreadsheetGear2017.Design.dll</HintPath>
		</Reference>
		<Reference Include="SpreadsheetGear2017.Drawing">
		  <HintPath>Libs\SpreadsheetGear2017.Drawing.dll</HintPath>
		</Reference>
		<Reference Include="SpreadsheetGear2017.Windows.WPF">
		  <HintPath>Libs\SpreadsheetGear2017.Windows.WPF.dll</HintPath>
		</Reference>
		<Reference Include="SpreadsheetGear2017.Windows.WPF.Design">
		  <HintPath>Libs\SpreadsheetGear2017.Windows.WPF.Design.dll</HintPath>
		</Reference>
		<Reference Include="System" Condition="!$(Configuration.Contains('2025'))" />
		<Reference Include="System.Windows.Forms" Condition="!$(Configuration.Contains('2025'))" />
		<Reference Include="System.Xaml" Condition="!$(Configuration.Contains('2025'))" />
		<Reference Include="UIFrameworkServices">
		  <HintPath>$(ProgramW6432)\Autodesk\Revit $(RevitVersion)\UIFrameworkServices.dll</HintPath>
		</Reference>
		<Reference Include="WindowsBase" Condition="!$(Configuration.Contains('2025'))" />
	</ItemGroup>
	
	<ItemGroup>
		<None Include="Project1\MVVM\ViewModel\Project1ViewModel.cs" />
		<None Include="ZRevitAPI\App.cs" />
		<None Include="Project1\RevitAPI\Project1Command.cs" />
		<None Include="Project1\RevitAPI\Project1RevitHandler.cs" />
		<None Include="Project1\RevitAPI\Project1Controller.cs" />
	</ItemGroup>

	<ItemGroup>
		<Resource Include="Resources\3D.png" />
		<Resource Include="Resources\AboutNew_16x16.ico" />
		<Resource Include="Resources\AboutNew_32x32.ico" />
		<Resource Include="Resources\accessoryoption1.png" />
		<Resource Include="Resources\add-circle.png" />
		<Resource Include="Resources\add_32x32.png" />
		<Resource Include="Resources\AutoDimPca_16x16.ico" />
		<Resource Include="Resources\AutoDimPca_32x32.ico" />
		<Resource Include="Resources\BeamBottom.png" />
		<Resource Include="Resources\BeamMiddle.png" />
		<Resource Include="Resources\BeamRebar_16x16.png" />
		<Resource Include="Resources\BeamRebar_32x32.png" />
		<Resource Include="Resources\BeamTop.png" />
		<Resource Include="Resources\bin.png" />
		<Resource Include="Resources\check-circle.png" />
		<Resource Include="Resources\CLOff_16x16.ico" />
		<Resource Include="Resources\CLOff_32x32.ico" />
		<Resource Include="Resources\CLOn_16x16.ico" />
		<Resource Include="Resources\CLOn_32x32.ico" />
		<Resource Include="Resources\ColumnRebar_16x16.png" />
		<Resource Include="Resources\ColumnRebar_32x32.png" />
		<Resource Include="Resources\concreteoption1.png" />
		<Resource Include="Resources\CouplerChecker_16x16.png" />
		<Resource Include="Resources\CouplerChecker_32x32.png" />
		<Resource Include="Resources\Coupler_16x16.png" />
		<Resource Include="Resources\Coupler_32x32.png" />
		<Resource Include="Resources\CreateGrid_16x16.png" />
		<Resource Include="Resources\CreateGrid_32x32.png" />
		<Resource Include="Resources\CUnlock_16x16.ico" />
		<Resource Include="Resources\CUnlock_32x32.ico" />
		<Resource Include="Resources\Grid16x16.ico" />
		<Resource Include="Resources\Grid32x32.ico" />
		<Resource Include="Resources\GridOff_16x16.png" />
		<Resource Include="Resources\GridOff_32x32.png" />
		<Resource Include="Resources\GridOn_16x16.png" />
		<Resource Include="Resources\GridOn_32x32.png" />
		<Resource Include="Resources\Image_type1.png" />
		<Resource Include="Resources\Image_type10.png" />
		<Resource Include="Resources\Image_type11.png" />
		<Resource Include="Resources\Image_type2.png" />
		<Resource Include="Resources\Dimension_16x16.png" />
		<Resource Include="Resources\Dimension_32x32.png" />
		<Resource Include="Resources\drawschedule16.png" />
		<Resource Include="Resources\DuplicateAssembly_16x16.png" />
		<Resource Include="Resources\DuplicateAssembly_32x32.png" />
		<Resource Include="Resources\duplicate_32x32.png" />
		<Resource Include="Resources\exportexcel16.png" />
		<Resource Include="Resources\gotosheet.png" />
		<Resource Include="Resources\gotosheet_32.png" />
		<Resource Include="Resources\HookStirrup_16x16.png" />
		<Resource Include="Resources\HookStirrup_32x32.png" />
		<Resource Include="Resources\icon.ico" />
		<Resource Include="Resources\Image_type3.png" />
		<Resource Include="Resources\Image_type4.png" />
		<Resource Include="Resources\Image_type5.png" />
		<Resource Include="Resources\Image_type6.png" />
		<Resource Include="Resources\Image_type7.png" />
		<Resource Include="Resources\Image_type8.png" />
		<Resource Include="Resources\Image_type9.png" />
		<Resource Include="Resources\Language_16x16.ico" />
		<Resource Include="Resources\Language_32x32.ico" />
		<Resource Include="Resources\LoadFamily_16x16.png" />
		<Resource Include="Resources\LoadFamily_32x32.png" />
		<Resource Include="Resources\loadpreset16.png" />
		<Resource Include="Resources\Lock_16x16.ico" />
		<Resource Include="Resources\Lock_32x32.ico" />
		<Resource Include="Resources\log-in.png" />
		<Resource Include="Resources\Logo_16x16.png" />
		<Resource Include="Resources\Logo_32x32.png" />
		<Resource Include="Resources\Matbang.png" />
		<Resource Include="Resources\MatCat.png" />
		<Resource Include="Resources\MEI_16x16.png" />
		<Resource Include="Resources\MEI_32x32.png" />
		<Resource Include="Resources\minus-circle.png" />
		<Resource Include="Resources\Ngosen_16x16.png" />
		<Resource Include="Resources\Ngosen_32x32.png" />
		<Resource Include="Resources\Note_1.png" />
		<Resource Include="Resources\Note_2.png" />
		<Resource Include="Resources\Note_3.png" />
		<Resource Include="Resources\Note_4.png" />
		<Resource Include="Resources\Off_16x16.png" />
		<Resource Include="Resources\Off_32x32.png" />
		<Resource Include="Resources\On_16x16.png" />
		<Resource Include="Resources\On_32x32.png" />
		<Resource Include="Resources\Opening_16x16.png" />
		<Resource Include="Resources\Opening_32x32.png" />
		<Resource Include="Resources\PCaSchedule_16x16.png" />
		<Resource Include="Resources\PCaSchedule_32x32.png" />
		<Resource Include="Resources\PCa_16x16.ico" />
		<Resource Include="Resources\PCa_32x32.ico" />
		<Resource Include="Resources\Project1_32x32.ico" />
		<Resource Include="Resources\Project1_16x16.ico" />
		<Resource Include="Resources\RebarCount_16x16.ico" />
		<Resource Include="Resources\RebarCount_32x32.ico" />
		<Resource Include="Resources\RebarDetail_16x16.png" />
		<Resource Include="Resources\RebarDetail_32x32.png" />
		<Resource Include="Resources\rebaroption1.png" />
		<Resource Include="Resources\rebaroption2.png" />
		<Resource Include="Resources\RebarSection_16x16.ico" />
		<Resource Include="Resources\RebarSection_32x32.ico" />
		<Resource Include="Resources\RebarSetting_16x16.png" />
		<Resource Include="Resources\RebarSetting_32x32.png" />
		<Resource Include="Resources\RebarStirrups_Note_0.png" />
		<Resource Include="Resources\RebarStirrups_Note_1.png" />
		<Resource Include="Resources\RebarStirrups_Note_2.png" />
		<Resource Include="Resources\RebarStirrups_Note_3.png" />
		<Resource Include="Resources\ReinforcingLeft.ico" />
		<Resource Include="Resources\ReinforcingLeft.png" />
		<Resource Include="Resources\ReinforcingRight.ico" />
		<Resource Include="Resources\RemoveRebarSection_16x16.ico" />
		<Resource Include="Resources\RemoveRebarSection_32x32.ico" />
		<Resource Include="Resources\remove_32x32.png" />
		<Resource Include="Resources\savepreset16.png" />
		<Resource Include="Resources\SectionUpdate_16x16.png" />
		<Resource Include="Resources\SectionUpdate_32x32.png" />
		<Resource Include="Resources\select16.png" />
		<Resource Include="Resources\selectschedule16.png" />
		<Resource Include="Resources\SettingTool_16x16.png" />
		<Resource Include="Resources\SettingTool_32x32.png" />
		<Resource Include="Resources\Showhideribbon_16x16.png" />
		<Resource Include="Resources\Showhideribbon_32x32.png" />
		<Resource Include="Resources\Stirrup_16x16.png" />
		<Resource Include="Resources\Stirrup_32x32.png" />
		<Resource Include="Resources\trash-circle.png" />
		<Resource Include="Resources\Unlock_16x16.png" />
		<Resource Include="Resources\Unlock_32x32.png" />
		<Resource Include="Resources\VerfiyModel_16x16.png" />
		<Resource Include="Resources\VerfiyModel_32x32.png" />
		<Resource Include="Resources\Whodidthis_16x16.png" />
		<Resource Include="Resources\Whodidthis_32x32.png" />
		<Page Include="0.Helper\CSHelper\MVVM\Components\View\InputComponentUserControl.xaml">
		  <Generator>MSBuild:Compile </ Generator >

        </ Page >

        < Page Include = "AutoDim\MVVM\View\AutoDimUserControl.xaml" >

          < Generator > MSBuild:Compile </ Generator >

        </ Page >

        < Page Include = "AutoDim\MVVM\View\ConfigDimUserControl.xaml" >

          < Generator > MSBuild:Compile </ Generator >

        </ Page >

        < Page Include = "CloudLocker\View\UI_LockTool.xaml" >

          < Generator > XamlIntelliSenseFileGenerator </ Generator >

          < CopyToOutputDirectory > Never </ CopyToOutputDirectory >

        </ Page >

        < Page Include = "Configuation\MVVM\View\ConfiguationUserControl.xaml" >

          < Generator > XamlIntelliSenseFileGenerator </ Generator >

          < CopyToOutputDirectory > Never </ CopyToOutputDirectory >

        </ Page >

        < Page Include = "Configuation\MVVM\View\MsgViewUserControl.xaml" >

          < CopyToOutputDirectory > Never </ CopyToOutputDirectory >

          < Generator > XamlIntelliSenseFileGenerator </ Generator >

        </ Page >

        < Page Include = "CouplerChecker\MVVM\View\CouplerCheckerUserControl.xaml" >

          < CopyToOutputDirectory > Never </ CopyToOutputDirectory >

          < Generator > XamlIntelliSenseFileGenerator </ Generator >

        </ Page >

        < Page Include = "CouplerChecker\MVVM\View\ProgressBarView.xaml" >

          < Generator > XamlIntelliSenseFileGenerator </ Generator >

          < CopyToOutputDirectory > Never </ CopyToOutputDirectory >

        </ Page >

        < Page Include = "DimensionRebar\MVVM\View\DimConfig.xaml" >

          < Generator > XamlIntelliSenseFileGenerator </ Generator >

          < CopyToOutputDirectory > Never </ CopyToOutputDirectory >

        </ Page >

        < Page Include = "DuplicateAssembly\MVVM\View\DuplicateAssemblyUserControl.xaml" >

          < Generator > XamlIntelliSenseFileGenerator </ Generator >

          < CopyToOutputDirectory > Never </ CopyToOutputDirectory >

        </ Page >

        < Page Include = "DuplicateAssembly\MVVM\View\SetMirrorSectionUserControl.xaml" >

          < Generator > XamlIntelliSenseFileGenerator </ Generator >

          < CopyToOutputDirectory > Never </ CopyToOutputDirectory >

        </ Page >

        < Page Include = "0.Helper\CSHelper\MVVM\Components\View\StatusComponentUserControl.xaml" >

          < Generator > MSBuild:Compile </ Generator >

        </ Page >

        < Page Include = "0.Helper\CSHelper\ProgressBar\ProgressBarView.xaml" >

          < CopyToOutputDirectory > Never </ CopyToOutputDirectory >

          < Generator > XamlIntelliSenseFileGenerator </ Generator >

        </ Page >

        < Page Include = "HideGridLevel\MVVM\View\HideGridLevelUserControl.xaml" >

          < SubType > Designer </ SubType >

          < Generator > MSBuild:Compile </ Generator >

        </ Page >

        < Page Include = "MoveRebarSection\MVVM\View\MoveRebarSectionUserControl.xaml" />

        < Page Include = "PCaSchedule\MVVM\View\LoadPresetUserControl.xaml" >

          < CopyToOutputDirectory > Never </ CopyToOutputDirectory >

          < SubType > Designer </ SubType >

          < Generator > XamlIntelliSenseFileGenerator </ Generator >

        </ Page >

        < Page Include = "PCaSchedule\MVVM\View\SavePresetUserControl.xaml" >

          < CopyToOutputDirectory > Never </ CopyToOutputDirectory >

          < SubType > Designer </ SubType >

          < Generator > XamlIntelliSenseFileGenerator </ Generator >

        </ Page >

        < Page Include = "PCaSchedule\MVVM\View\SelectScheduleUserControl.xaml" >

          < CopyToOutputDirectory > Never </ CopyToOutputDirectory >

          < SubType > Designer </ SubType >

          < Generator > XamlIntelliSenseFileGenerator </ Generator >

        </ Page >

        < Page Include = "PCBeamRebar\MVVM\View\RebarTop.xaml" >

          < CopyToOutputDirectory > Never </ CopyToOutputDirectory >

          < Generator > XamlIntelliSenseFileGenerator </ Generator >

        </ Page >

        < Page Include = "PCBeamRebar\MVVM\View\OpenPreset.xaml" >

          < Generator > XamlIntelliSenseFileGenerator </ Generator >

          < CopyToOutputDirectory > Never </ CopyToOutputDirectory >

        </ Page >

        < Page Include = "PCBeamRebar\MVVM\View\PCaBeamRebarUserControl.xaml" >

          < Generator > XamlIntelliSenseFileGenerator </ Generator >

          < CopyToOutputDirectory > Never </ CopyToOutputDirectory >

        </ Page >

        < Page Include = "PCBeamRebar\MVVM\View\SavePreset.xaml" >

          < Generator > XamlIntelliSenseFileGenerator </ Generator >

          < CopyToOutputDirectory > Never </ CopyToOutputDirectory >

        </ Page >

        < Page Include = "PCaSchedule\MVVM\View\PCaScheduleUserControl.xaml" >

          < SubType > Designer </ SubType >

          < Generator > XamlIntelliSenseFileGenerator </ Generator >

          < CopyToOutputDirectory > Never </ CopyToOutputDirectory >

        </ Page >

        < Page Include = "Project1\MVVM\View\Project1UserControl.xaml" >

          < Generator > MSBuild:Compile </ Generator >

        </ Page >

        < Page Include = "RebarConfig\MVVM\View\RebarConfigUserControl.xaml" >

          < Generator > XamlIntelliSenseFileGenerator </ Generator >

          < CopyToOutputDirectory > Never </ CopyToOutputDirectory >

        </ Page >

        < Page Include = "RebarCounter\MVVM\View\RebarCounterUserControl.xaml" >

          < Generator > XamlIntelliSenseFileGenerator </ Generator >

          < CopyToOutputDirectory > Never </ CopyToOutputDirectory >

        </ Page >

        < Page Include = "RebarHoisting\MVVM\View\RebarHoistingUserControl.xaml" >

          < CopyToOutputDirectory > Never </ CopyToOutputDirectory >

          < Generator > XamlIntelliSenseFileGenerator </ Generator >

        </ Page >

        < Page Include = "RebarOpenning\MVVM\View\RebarOpenningUserControl.xaml" >

          < Generator > XamlIntelliSenseFileGenerator </ Generator >

          < CopyToOutputDirectory > Never </ CopyToOutputDirectory >

        </ Page >

        < Page Include = "RebarSectionSetting\MVVM\View\RebarSectionSettingUserControl.xaml" >

          < Generator > XamlIntelliSenseFileGenerator </ Generator >

          < CopyToOutputDirectory > Never </ CopyToOutputDirectory >

        </ Page >

        < Page Include = "RebarStirrups\MVVM\View\RebarStirrupsSaveUserControl.xaml" >

          < Generator > XamlIntelliSenseFileGenerator </ Generator >

          < CopyToOutputDirectory > Never </ CopyToOutputDirectory >

        </ Page >

        < Page Include = "RebarStirrups\MVVM\View\RebarStirrupsUserControl.xaml" >

          < CustomToolNamespace ></ CustomToolNamespace >

          < CopyToOutputDirectory > Never </ CopyToOutputDirectory >

          < Generator > XamlIntelliSenseFileGenerator </ Generator >

        </ Page >

        < Page Include = "RebarStirrups\MVVM\View\RebarStirrupTemplatesUserControl.xaml" >

          < CustomToolNamespace ></ CustomToolNamespace >

          < CopyToOutputDirectory > Never </ CopyToOutputDirectory >

          < Generator > XamlIntelliSenseFileGenerator </ Generator >

        </ Page >

        < Page Include = "ShowGridLevel\MVVM\View\ShowGridLevelUserControl.xaml" >

          < Generator > MSBuild:Compile </ Generator >

        </ Page >

        < Page Include = "ShowHideRibbon\MVVM\View\ShowHideRibbonUserControl.xaml" >

          < SubType > Designer </ SubType >

          < Generator > MSBuild:Compile </ Generator >

        </ Page >

        < Page Include = "Style\Styles.xaml" >

          < CopyToOutputDirectory > Never </ CopyToOutputDirectory >

          < Generator > XamlIntelliSenseFileGenerator </ Generator >

        </ Page >

        < Page Include = "Style\Themes.xaml" >

          < SubType > Designer </ SubType >

          < Generator > XamlIntelliSenseFileGenerator </ Generator >

          < CopyToOutputDirectory > Never </ CopyToOutputDirectory >

        </ Page >

        < Page Include = "VerifyModel\UI\View\ExportModelInformationDlg.xaml" >

          < Generator > XamlIntelliSenseFileGenerator </ Generator >

          < CopyToOutputDirectory > Never </ CopyToOutputDirectory >

        </ Page >

        < Page Include = "VerifyModel\UI\View\ModelsCheckDlg.xaml" >

          < Generator > XamlIntelliSenseFileGenerator </ Generator >

          < CopyToOutputDirectory > Never </ CopyToOutputDirectory >

        </ Page >

        < Page Include = "VerifyModel\UI\View\ProgressBarView.xaml" >

          < Generator > XamlIntelliSenseFileGenerator </ Generator >

          < CopyToOutputDirectory > Never </ CopyToOutputDirectory >

        </ Page >

        < Page Include = "WhoDidThisV1\MVVM\View\WhoDidThisV1UserControl.xaml" >

          < Generator > XamlIntelliSenseFileGenerator </ Generator >

          < CopyToOutputDirectory > Never </ CopyToOutputDirectory >

        </ Page >

        < Page Include = "WhoDidThisV2\MVVM\View\WhoDidThisV2UserControl.xaml" >

          < Generator > XamlIntelliSenseFileGenerator </ Generator >

          < CopyToOutputDirectory > Never </ CopyToOutputDirectory >

        </ Page >

    </ ItemGroup >


    < ItemGroup >

        < Content Include = "PCTools.addin" >

            < SubType > Designer </ SubType >

        </ Content >

    </ ItemGroup >


    < ItemGroup >

      < EmbeddedResource Include = "Resources\checkforupdatePCA.json" />

      < EmbeddedResource Include = "Resources\Grid_XCheck_2022.rfa" />

      < EmbeddedResource Include = "Resources\Grid_XCheck_2023.rfa" />

      < EmbeddedResource Include = "Resources\Grid_XCheck_2024.rfa" />

      < EmbeddedResource Include = "Resources\Grid_XCheck_2025.rfa" />

      < EmbeddedResource Include = "Resources\Grid_YCheck_2022.rfa" />

      < EmbeddedResource Include = "Resources\Grid_YCheck_2023.rfa" />

      < EmbeddedResource Include = "Resources\Grid_YCheck_2024.rfa" />

      < EmbeddedResource Include = "Resources\Grid_YCheck_2025.rfa" />

      < EmbeddedResource Include = "Resources\LevelCheck_2022.rfa" />

      < EmbeddedResource Include = "Resources\LevelCheck_2023.rfa" />

      < EmbeddedResource Include = "Resources\LevelCheck_2024.rfa" />

      < EmbeddedResource Include = "Resources\LevelCheck_2025.rfa" />

      < EmbeddedResource Include = "Resources\MFUpdate.exe" />

      < EmbeddedResource Include = "Resources\M_00_2023.rfa" />

      < EmbeddedResource Include = "Resources\M_00_2024.rfa" />

      < EmbeddedResource Include = "Resources\M_00_2025.rfa" />

      < EmbeddedResource Include = "Resources\M_01_2023.rfa" />

      < EmbeddedResource Include = "Resources\M_01_2024.rfa" />

      < EmbeddedResource Include = "Resources\M_01_2025.rfa" />

      < EmbeddedResource Include = "Resources\PcaSchedule.txt" />

      < EmbeddedResource Include = "Resources\PCaSharedParamter.txt" />

      < EmbeddedResource Include = "Resources\PCA_Grid.rfa" />

      < EmbeddedResource Include = "Resources\PCA_Grid_2023.rfa" />

      < EmbeddedResource Include = "Resources\PCA_Grid_2024.rfa" />

      < EmbeddedResource Include = "Resources\PCA_Grid_2025.rfa" />

      < EmbeddedResource Include = "Resources\PCA_Grid_Y.rfa" />

      < EmbeddedResource Include = "Resources\PCA_Grid_Y_2023.rfa" />

      < EmbeddedResource Include = "Resources\PCA_Grid_Y_2024.rfa" />

      < EmbeddedResource Include = "Resources\PCA_Grid_Y_2025.rfa" />

      < EmbeddedResource Include = "Resources\PCa_RebarSection_2023.rfa" />

      < EmbeddedResource Include = "Resources\PCa_RebarSection_2024.rfa" />

      < EmbeddedResource Include = "Resources\PCa_RebarSection_2025.rfa" />

      < EmbeddedResource Include = "Resources\PCa_RebarSection_type10_2023.rfa" />

      < EmbeddedResource Include = "Resources\PCa_RebarSection_type10_2024.rfa" />

      < EmbeddedResource Include = "Resources\PCa_RebarSection_type10_2025.rfa" />

      < EmbeddedResource Include = "Resources\PCa_RebarSection_type11_2023.rfa" />

      < EmbeddedResource Include = "Resources\PCa_RebarSection_type11_2024.rfa" />

      < EmbeddedResource Include = "Resources\PCa_RebarSection_type11_2025.rfa" />

      < EmbeddedResource Include = "Resources\PCa_RebarSection_type1_2023.rfa" />

      < EmbeddedResource Include = "Resources\PCa_RebarSection_type1_2024.rfa" />

      < EmbeddedResource Include = "Resources\PCa_RebarSection_type1_2025.rfa" />

      < EmbeddedResource Include = "Resources\PCa_RebarSection_type2_2023.rfa" />

      < EmbeddedResource Include = "Resources\PCa_RebarSection_type2_2024.rfa" />

      < EmbeddedResource Include = "Resources\PCa_RebarSection_type2_2025.rfa" />

      < EmbeddedResource Include = "Resources\PCa_RebarSection_type3_2023.rfa" />

      < EmbeddedResource Include = "Resources\PCa_RebarSection_type3_2024.rfa" />

      < EmbeddedResource Include = "Resources\PCa_RebarSection_type3_2025.rfa" />

      < EmbeddedResource Include = "Resources\PCa_RebarSection_type4_2023.rfa" />

      < EmbeddedResource Include = "Resources\PCa_RebarSection_type4_2024.rfa" />

      < EmbeddedResource Include = "Resources\PCa_RebarSection_type4_2025.rfa" />

      < EmbeddedResource Include = "Resources\PCa_RebarSection_type5_2023.rfa" />

      < EmbeddedResource Include = "Resources\PCa_RebarSection_type5_2024.rfa" />

      < EmbeddedResource Include = "Resources\PCa_RebarSection_type5_2025.rfa" />

      < EmbeddedResource Include = "Resources\PCa_RebarSection_type6_2023.rfa" />

      < EmbeddedResource Include = "Resources\PCa_RebarSection_type6_2024.rfa" />

      < EmbeddedResource Include = "Resources\PCa_RebarSection_type6_2025.rfa" />

      < EmbeddedResource Include = "Resources\PCa_RebarSection_type7_2023.rfa" />

      < EmbeddedResource Include = "Resources\PCa_RebarSection_type7_2024.rfa" />

      < EmbeddedResource Include = "Resources\PCa_RebarSection_type7_2025.rfa" />

      < EmbeddedResource Include = "Resources\PCa_RebarSection_type8_2023.rfa" />

      < EmbeddedResource Include = "Resources\PCa_RebarSection_type8_2024.rfa" />

      < EmbeddedResource Include = "Resources\PCa_RebarSection_type8_2025.rfa" />

      < EmbeddedResource Include = "Resources\PCa_RebarSection_type9_2023.rfa" />

      < EmbeddedResource Include = "Resources\PCa_RebarSection_type9_2024.rfa" />

      < EmbeddedResource Include = "Resources\PCa_RebarSection_type9_2025.rfa" />

      < EmbeddedResource Include = "Resources\StirrupTemplate_2023.rfa" />

      < EmbeddedResource Include = "Resources\StirrupTemplate_2024.rfa" />

    </ ItemGroup >


    < ItemGroup >

      < PackageReference Include = "FireSharp" Version = "2.0.4" />

      < PackageReference Include = "Microsoft.CSharp" Version = "4.7.0" />

      < PackageReference Include = "Newtonsoft.Json" Version = "13.0.3" />

    </ ItemGroup >


    < ItemGroup >

      < Compile Update = "Properties\Resources_en-US.Designer.cs" >

        < DesignTime > True </ DesignTime >

        < AutoGen > True </ AutoGen >

        < DependentUpon > Resources_en - US.resx </ DependentUpon >

      </ Compile >

      < Compile Update = "Properties\Resources_ja-JP.Designer.cs" >

        < DesignTime > True </ DesignTime >

        < AutoGen > True </ AutoGen >

        < DependentUpon > Resources_ja - JP.resx </ DependentUpon >

      </ Compile >

      < Compile Update = "Properties\Resources_vi-VN.Designer.cs" >

        < DesignTime > True </ DesignTime >

        < AutoGen > True </ AutoGen >

        < DependentUpon > Resources_vi - VN.resx </ DependentUpon >

      </ Compile >

      < Compile Update = "Properties\Settings.Designer.cs" >

        < DesignTimeSharedInput > True </ DesignTimeSharedInput >

        < AutoGen > True </ AutoGen >

        < DependentUpon > Settings.settings </ DependentUpon >

      </ Compile >

    </ ItemGroup >


    < ItemGroup >

      < EmbeddedResource Update = "Properties\Resources_en-US.resx" >

        < Generator > ResXFileCodeGenerator </ Generator >

        < LastGenOutput > Resources_en - US.Designer.cs </ LastGenOutput >

      </ EmbeddedResource >

      < EmbeddedResource Update = "Properties\Resources_ja-JP.resx" >

        < Generator > ResXFileCodeGenerator </ Generator >

        < LastGenOutput > Resources_ja - JP.Designer.cs </ LastGenOutput >

      </ EmbeddedResource >

      < EmbeddedResource Update = "Properties\Resources_vi-VN.resx" >

        < Generator > ResXFileCodeGenerator </ Generator >

        < LastGenOutput > Resources_vi - VN.Designer.cs </ LastGenOutput >

      </ EmbeddedResource >

    </ ItemGroup >


    < ItemGroup >

      < None Update = "Properties\Settings.settings" >

        < Generator > SettingsSingleFileGenerator </ Generator >

        < LastGenOutput > Settings.Designer.cs </ LastGenOutput >

      </ None >

    </ ItemGroup >


    < PropertyGroup Condition = "$(Configuration.Contains('Debug'))" >

        < PostBuildEvent >
            SET pathAddin =$(AppData)\Autodesk\REVIT\Addins\$(RevitVersion)\$(AssemblyName)\
			if not exist "%pathAddin%" mkdir "%pathAddin%"
			echo "****************************************** DEBUG ******************************************"
			if exist "$(AppData)\Autodesk\REVIT\Addins\$(RevitVersion)" copy "$(SolutionDir)$(AssemblyName)\*.addin" "$(AppData)\Autodesk\REVIT\Addins\$(RevitVersion)"
			if exist "$(AppData)\Autodesk\REVIT\Addins\$(RevitVersion)\$(AssemblyName)\" copy "$(SolutionDir)$(AssemblyName)\$(OutputPath)*.dll" "$(AppData)\Autodesk\REVIT\Addins\$(RevitVersion)\$(AssemblyName)\"
			cd $(TargetDir)
			del  RevitAPI.dll RevitAPIUI.dll
			del  *.pdb *.config *.xml *.hash
			rmdir /s /q "x64"
			rmdir /s /q "x86"
		</PostBuildEvent>
	</PropertyGroup>
	<PropertyGroup Condition="!$(Configuration.Contains('Debug'))">
		<PostBuildEvent>
			set FILES=AdWindows.dll RevitAPI.dll RevitAPIUI.dll
			for %%i in (%FILES%) do (
			  if exist "$(ProgramW6432)\Autodesk\Revit $(RevitVersion)\%%i" copy "$(ProgramW6432)\Autodesk\Revit $(RevitVersion)\%%i" "$(SolutionDir)$(AssemblyName)\$(OutputPath)\"
			)
			SET pathdll=$(SolutionDir)$(AssemblyName)\$(OutputPath)
			echo "****************************************** RELEASE ******************************************"
			IF EXIST "C:\Program Files (x86)\Eziriz\.NET Reactor\dotNET_Reactor.Console.exe" (
			  "C:\Program Files (x86)\Eziriz\.NET Reactor\dotNET_Reactor.Console.exe" -file "$(SolutionDir)$(AssemblyName)\$(OutputPath)\$(AssemblyName).dll" -targetfile "&lt;AssemblyLocation&gt;\&lt;AssemblyFileName&gt;" -hide_calls 1 -control_flow 1 -flow_level 9 -suppressildasm 0
			)
			cd $(pathdll)
			for %%i in (%FILES%) do (
			del %%i
			)
			del  *.pdb *.config *.xml *.hash
			rmdir /s /q "x64"
			rmdir /s /q "x86"
			copy "$(SolutionDir)$(AssemblyName)\*.addin" "$(SolutionDir)$(AssemblyName)\bin\Release\$(RevitVersion)\"
		</PostBuildEvent>
	</PropertyGroup>
	<Target Name="CleanRevitAddin" BeforeTargets="CoreClean">
		<Message Text="Clean Revit Addin Files" Importance="high" />
		<Delete Files="$(AppData)\Autodesk\REVIT\Addins\$(RevitVersion)\$(AssemblyName).addin" />
		<Delete Files="$(AppData)\Autodesk\REVIT\Addins\$(RevitVersion)\$(AssemblyName).dll" />
	</Target>
</Project>