# TODO LIST

> # Sql tables / View
> - [ ] CREATE VIEW nbr total sum by month / year and year by place

> # Function
> - [X] Recursive expenses ![100%](https://geps.dev/progress/100)<!-- [3/3] -->
>   - [X] Add new table Sql for recursive expense with all details / definition of time
>   - [X] Add Method to add / edit Sql table with application
>   - [X] At each start of database with application, prompt message to the user to ask if he wants to add all recurse expenses for the current month

> # Interface
>  - [ ] Create ToolBar with item ![0%](https://geps.dev/progress/0)<!-- [0/2] -->
>    - [ ] File ![33%](https://geps.dev/progress/33) <!-- [1/3] -->
>      - [ ] Import database ![40%](https://geps.dev/progress/40) <!-- [2/5] -->
>        - [X] From database file
>        - [X] From cloud ([API Dropbox](https://www.dropbox.com/developers/documentation/http/documentation)) (database file format only)
>        - [ ] From xlsx format (.shp/.kml/.kmz if geometry data)
>        - [ ] From ods format (.shp/.kml/.kmz if geometry data) (not required because LibreOffice can read xlsx files)
>        - [ ] From csv format (.shp/.kml/.kmz if geometry data)
>      - [ ] Export database ![50%](https://geps.dev/progress/50) <!-- [2/4] -->
>        - [X] To database format
>        - [X] To cloud ([API Dropbox](https://www.dropbox.com/developers/documentation/http/documentation)) (database file format only)
>        - [ ] To xlsx format (.shp/.kml/.kmz if geometry data with Qgis project (.qgs/.qgz)) ![0%](https://geps.dev/progress/0)<!-- [0/6] -->
>          - [ ] .xlsx
>          - [ ] .shp (.shx, .dbf)
>          - [ ] .kml
>          - [ ] .kmz
>          - [ ] .qgs
>          - [ ] .qgz
>        - [ ] To csv format (.shp/.kml/.kmz if geometry data with Qgis project (.qgs/.qgz)) ![50%](https://geps.dev/progress/50)<!-- [3/6] -->
>          - [X] .csv
>          - [ ] .shp (.shx, .dbf)
>          - [X] .kml
>          - [X] .kmz
>          - [ ] .qgs
>          - [ ] .qgz
>      - [X] Clean database (SQL query `VACUUM;`)
>    - [ ] Help ![0%](https://geps.dev/progress/0) <!-- [0/2] -->
>      - [ ] Tutorial (MultiLanguage) 
>      - [ ] Version

> # Other
> - [ ] added error tracking / fix / added function
> - [ ] Auto updater with GitHub