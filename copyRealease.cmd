ECHO D | XCOPY "UpdateCompactControl\bin\Release" "Release-%DATE%" /EXCLUDE:exclude.txt

IF EXIST Release-%DATE%.zip DEL Release-%DATE%.zip