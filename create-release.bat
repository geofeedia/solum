@echo off
REM xcopy /F /D /S /Y bin\* release\%current_date%\bin\

REM -- Store the current path
set orig_path=%cd%

REM -- Get the current date time in the format YYYY-MM-DD
set current_date=%DATE:~10,4%-%DATE:~4,2%-%DATE:~7,2%
set release_dir=release\current

REM -- Update with contents from bin folder and copy to release directory
lib\utils\echoc 2 Creating release binaries...
mkdir %release_dir%\bin\
lib\utils\echoc 2 Copying \bin...
robocopy bin %release_dir%\bin /MIR /XD data logs

REM -- Create lib
lib\utils\echoc 2 Creating release lib...
mkdir %release_dir%\lib\
lib\ILMerge\ILMerge.exe /v4 /out:%release_dir%\lib\solumlib.dll /wildcards bin\solum.*.dll bin\solum.exe bin\RaptorDB*.dll bin\Handlebars.dll bin\System.Threading.Tasks.Dataflow.dll

REM -- Compress bin and lib to specific zip files
lib\utils\echoc 2 Compressing binary release...
del %release_dir%\..\%current_date%-solum-bin.zip
cd %release_dir%
%orig_path%\lib\utils\zip -r ..\%current_date%-solum-bin.zip bin\*
cd %orig_path%

lib\utils\echoc 2 Compressing lib release...
del %release_dir%\..\%current_date%-solum-lib.zip
cd %release_dir%
%orig_path%\lib\utils\zip -r ..\%current_date%-solum-lib.zip lib\*
cd %orig_path%

REM -- Update DataCollectors if present
if exist ..\datacollectors\lib (
	echo.
	lib\utils\echoc 2 Found data collectors project...
	set updatedclib=y
	set /p updatedclib="Update data collectors solum library? [y]/n "

	if "%updatedclib:~0,1%" equ "y" (
		lib\utils\echoc 2 Removing existing solum library archive...
		del ..\datacollectors\lib\*-solum*.zip
		
		lib\utils\echoc 2 Copying release\%current_date%-solum-lib.zip...
		copy /y /v release\%current_date%-solum-lib.zip ..\datacollectors\lib\
		
		lib\utils\echoc 2 Extracting library...
		lib\utils\unzip -o ..\datacollectors\lib\*-solum-lib.zip -d ..\datacollectors\lib\solum
	) else (
		goto GIT
	)
)


:GIT
REM -- Display git status
git status

REM -- Git Add
echo.
set git_add=y
set /p git_add="Git add release? [y]/n "

if "%git_add:~0,1%" equ "y" (
	lib\utils\echoc 2 Adding...
	git add -Af .\release\*
) else (
	goto COMPLETED
)

git status

REM -- Git Commmit
echo.
set git_commit=y
set /p git_commit="Git commit? [y]/n "

if "%git_commit:~0,1%" equ "y" (
	lib\utils\echoc 2 Commiting...
	git commit -am "Created release %current_date%."
) else (
	goto COMPLETED
)

REM -- Git Push
echo.
set git_push=y
set /p git_push="Git push? [y]/n "

if "%git_push:~0,1%" equ "y" (
	lib\utils\echoc 2 Pushing...
	git push
) else (
	goto COMPLETED
)

echo.
git status

:COMPLETED
echo.
lib\utils\echoc 2 Sucessfully created release %current_date%.