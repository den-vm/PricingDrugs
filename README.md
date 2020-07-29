# Description in Russian language
## Ценообразование препаратов
Данное приложение предназначено для расчёта цен на препараты реестра ЖВНЛП в регионе Республика Бурятия Российской Федерации 

## Структура репозитория
* Проект (папка ServiceJob)
* Компиляция под Linux и Windows (папка Publish)
* Компиляция под Windows 10 X64 (папка win-x64)

## Структура проекта
* Frontend (Сайт) - папка wwwroot
* Backend (Приложение) - используемые библиотеки .dll и исполняемый файл .exe
* Используемый стек технологий - Net Core 2.1
* Используемые NuGet пакеты - AlertifyJS, EPPlus.Core, ExcelDataReader.DataSet, Newtonsoft.Json

## Запуск
Запуск приложения осуществляется с помощью исполняемого файла ServiceJob.exe или с помощью командной строки "dotnet ServiceJob.dll". Сайт доступен по адресу https://localhost:5001/.

## Настройка
Начальная настройка приложения заключается в указании последней даты обновления или любой даты в файле lastdateupdate.json, заполнения таблицы наркотических и психотропных препаратов и критериев расчёта на сайте по адресу https://localhost:5001/Jvnlp в вкладках "Таблица нарк. и псих. препаратов" и "Таблица критерий на цены нарк. и псих. препаратов"

Файл "lastdateupdate.json" хранит последнюю дату обновления реестра, которая в последующем необходима будет в формировании реестра включенных препаратов за период между последней даты обновления и текущей.

Файл "configcriteria.json" хранит критерии расчёта препаратов в %, которые включают в себя не наркотические и наркотические, не психотропные и психотропные препараты, а также НДС.

Файл "JVNLP_.xlsx" является шаблоном для последующего формирования и сохранения рассчитанного реестра в файл JVNLP_XX_XX_XXXX.xlsx.

## Выполнение расчёта
Предварительно в вкладках "Таблица нарк. и псих. препаратов" и "Таблица критерий на цены нарк. и псих. препаратов" не должны быть пустыми.
1) Необходимо скачать реестр ЖВНЛП по адресу http://grls.rosminzdrav.ru/pricelims.aspx, скачать файл и распаковать его;
2) Зайти на сайт по адресу https://localhost:5001/ и перейти на вкладку "ЖВНЛП";
3) Открыть вкладку "Таблица ЖВНЛП" и загрузить реестр (распакованный файл);
4) Открыть вкладку "Рассчитанный реестр" и нажать на кнопку "Рассчитать" для выполнения расчёта;
5) Нажать кнопку "Сохранить файл" и выполнить сохранения рассчитанного реестра.


# Description in English language
## Drug pricing
This application is intended for calculating prices for medicines from the ZHVNLP registry in the Republic of Buryatia region of the Russian Federation

## Repository structure
* Project (ServiceJob folder)
* Compilation for Linux and Windows (Publish folder)
* Compilation for Windows 10 X64 (win-x64 folder)

## Project structure
* Frontend (Site) - the wwwroot folder
* Backend (Application) - used libraries .dll and .exe executable file
* The technology stack used is Net Core 2.1
* NuGet packages used-AlertifyJS, EPPlus.Core, ExcelDataReader.DataSet, Newtonsoft.Json

## Launch
The application is launched using an executable file ServiceJob.exe or using the "dotnet" command line ServiceJob.dll". The site is available at https://localhost:5001/.

## Customization
The initial configuration of the application is to specify the last update date or any date in the lastdateupdate file.json, filling in the table of narcotic and psychotropic drugs and calculation criteria on the site at https://localhost:5001/Jvnlp in the "narc table" tabs. and crazy. drugs " and " table of criteria for drug prices. and crazy. drugs"

File "lastdateupdate.json " stores the last update date of the registry, which will later be necessary in the formation of the registry of included drugs for the period between the last update date and the current one.

File "configcriteria.json " stores the criteria for calculating drugs in%, which include non-narcotic and narcotic drugs, non-psychotropic and psychotropic drugs, as well as VAT.

File "JVNLP_.xlsx" is a template for later generating and saving the calculated registry to a file JVNLP_XX_XX_XXXX.xlsx.

## Performance of calculation
Previously, in the tabs " table of drugs. and crazy. drugs " and " table of criteria for drug prices. and crazy. the words " should not be empty.
1) You need to download the register of ZHVNLP at http://grls.rosminzdrav.ru/pricelims.aspx, download the file and unpack it;
2) Go to the website at https://localhost:5001/ and go to the "ZHVNLP" tab";
3) Open the "ZHVNLP table" tab and load the registry (unpacked file);
4) Open the "Calculated registry" tab and click the "Calculate" button to perform the calculation;
5) Click the "Save file" button and save the calculated registry.
