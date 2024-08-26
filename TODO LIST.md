# TODO LIST

> # Sql tables / View
> - [ ] CREATE VIEW nbr total sum by month / year and year by place

> # Function
> - [X] Recursive expenses ![100%](https://geps.dev/progress/100)<!-- [3/3] -->
>   - [X] Add new table Sql for recursive expense with all details / definition of time
>   - [X] Add Method to add / edit Sql table with application
>   - [X] At each start of database with application, prompt message to the user to ask if he wants to add all recurse expenses for the current month

> # Interface
> - [ ] Create ToolBar with item ![0%](https://geps.dev/progress/0)<!-- [0/2] -->
>   - [ ] File ![33%](https://geps.dev/progress/33) <!-- [2/3] -->
>     - [ ] Import database ![40%](https://geps.dev/progress/40) <!-- [2/5] -->
>       - [X] From database file
>       - [X] From cloud ([API Dropbox](https://www.dropbox.com/developers/documentation/http/documentation)) (database file format only)
>       - [ ] From xlsx format (.shp/.kml/.kmz if geometry data)
>       - [ ] From ods format (.shp/.kml/.kmz if geometry data) (not required because LibreOffice can read xlsx files)
>       - [ ] From csv format (.shp/.kml/.kmz if geometry data)
>     - [ ] Export database ![50%](https://geps.dev/progress/50) <!-- [1/2] -->
>       - [X] To cloud ([API Dropbox](https://www.dropbox.com/developers/documentation/http/documentation)) (database file format only)
>       - [ ] Export to local file ![29%](https://geps.dev/progress/29) <!-- [2/7] -->
>         - [X] .sqlite
>         - [ ] .csv
>         - [X] .xlsx
>         - [ ] .shp (.shx, .dbf)
>         - [X] .kml
>         - [ ] .kmz
>         - [ ] .qgs
>         - [ ] .qgz
>     - [X] Clean database (SQL query `VACUUM;`)
>     - [ ] Help ![33%](https://geps.dev/progress/33) <!-- [1/3] -->
>       - [X] Version ![100%](https://geps.dev/progress/100) <!-- [3/3] -->
>         - [X] Application version
>         - [X] Database version
>         - [X] Sqlite version
>       - [ ] Changelog
>       - [ ] How to use (MultiLanguage) ![0%](https://geps.dev/progress/0)<!-- [0/3] -->
>         - [ ] English
>         - [ ] French
>         - [ ] Portuguese
> - [ ] Language ![66%](https://geps.dev/progress/66) <!-- [2/3] -->
>   - [X] English
>   - [X] French
>   - [ ] Portuguese

> # Other
> - [ ] added error tracking / fix / added function
> - [ ] Auto updater with GitHub