@echo off
set "folder=%temp%"
set "pattern=test-file-tasks*.bin"

echo Deleting test files in %temp% folder

for %%F in ("%folder%\%pattern%") do (
    echo Deleting "%%~nxF"
    del "%%F"
)

echo Deletion complete.
pause
