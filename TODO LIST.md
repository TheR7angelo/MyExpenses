# TODO LIST

> # Sql tables / View
> - [ ] CREATE VIEW nbr total sum by month / year and year by place

> # Function
> - [ ] Recursive expenses ![33%](https://progress-bar.dev/0)<!-- [1/3] -->
>   - [X] Add new table Sql for recursive expense with all details / definition of time
>   - [ ] Add Method to add / edit Sql table with application
>   - [ ] At each start of database with application, prompt message to the user to ask if he wants to add all recurse expenses for the current month

> # Interface
>  - [ ] Create ToolBar with item ![0%](https://progress-bar.dev/0)<!-- [0/2] -->
>    - [ ] File ![33%](https://progress-bar.dev/33) <!-- [1/3] -->
>      - [ ] Import database ![40%](https://progress-bar.dev/40) <!-- [2/5] -->
>        - [X] From database file
>        - [X] From cloud ([API Dropbox](https://www.dropbox.com/developers/documentation/http/documentation)) (database file format only)
>        - [ ] From xlsx format (.shp if geometry data)
>        - [ ] From ods format (.shp if geometry data) (not required because LibreOffice can read xlsx files)
>        - [ ] From csv format (.shp if geometry data)
>      - [ ] Export database ![40%](https://progress-bar.dev/40) <!-- [2/5] -->
>        - [X] To database format
>        - [X] To cloud ([API Dropbox](https://www.dropbox.com/developers/documentation/http/documentation)) (database file format only)
>        - [ ] To xlsx format (.shp if geometry data)
>        - [ ] To ods format (.shp if geometry data) (not required because LibreOffice can read xlsx files)
>        - [ ] To csv format (.shp if geometry data)
>      - [X] Clean database (SQL query `VACUUM;`)
>    - [ ] Help ![0%](https://progress-bar.dev/0) <!-- [0/2] -->
>      - [ ] Tutorial (MultiLanguage) 
>      - [ ] Version

> # Other
> - [ ] added error tracking / fix / added function
> - [ ] Auto updater with GitHub