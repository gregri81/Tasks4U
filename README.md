# Description

I wrote this WPF application for my wife - she needs a tasks management application for her job.
However, everybody is free to use it.

The idea is simple: You can add, edit and delete tasks. Behind the scenes, I implemented editing by deleting and adding.

Every task has the following hardcoded fields (according to my wife's request): 
- Mandatory - Subject, Frequency, Final Date, Desk, Status.
- Optional - Intermediate Date, Related To, Description.

The frequency may be:
- For recurring tasks - Every Week, Every Month, or Every Year
- For non-recurring tasks - Once

There is a timer that checks which tasks are due today (either according to the intermediate date or according to the final date). If the current time is 9 o'clock or later, a notification is shown (9 o'clock is hardcoded according to my wife's request). When the user clicks the notification, the corresponding task is shown.

I used a RichTextBox to edit the description, so it supports formatting (bold, underline, fonts, etc.).

It's possible to save the tasks list or a single task to a PDF file (I used PrintDialog for this purpose).
It's also possible to save the tasks list to an Excel file (I used the ClosedXML library for this purpose).

# Technology Stack
- As stated above, I used WPF.
- I used CommunityToolkit.Mvvm to adhere to the MVVM pattern. In some places, I broke the rules of MVVM to simplify the code - I am not a purist.
- I used an Sqlite database to store the tasks. I think that in a desktop application without a server, Sqlite is a good fit.
- I used EF Core 6 for database access.
- I Used the open-source ClosedXML library (https://github.com/ClosedXML/ClosedXML) to create and save an Excel file.

# Screenshots

Tasks Grid:
![TasksGrid](https://user-images.githubusercontent.com/116143087/233641618-147d3a70-8df9-418c-949f-c37402a17775.png)

Zoomed-In Tasks Grid:
![ZoomedInTasksGrid](https://user-images.githubusercontent.com/116143087/233645977-b2081de8-fd92-41cd-91f9-674888479187.png)

Filtered Tasks Grid:
![FilteredTaskGrid](https://user-images.githubusercontent.com/116143087/233646040-954e9821-e74c-47a5-9fd1-7f2ecaac0442.png)

Adding a Task:
![AddingTask](https://user-images.githubusercontent.com/116143087/233644015-17daf32e-4535-497f-b685-4df930db70a3.png)

Editing a Task:
![EditingTask](https://user-images.githubusercontent.com/116143087/233644079-4ef05426-cb4a-4863-9b24-cdbd23ea08cd.png)

Zoomed-In Task:
![ZoomedInTask](https://user-images.githubusercontent.com/116143087/233646490-03163a33-b27e-45f8-a3d1-8b2b01ceb671.png)

A Task Exported to PDF:
![TaskPdfExport](https://user-images.githubusercontent.com/116143087/233644190-06e8630f-d61a-48ea-ac9d-014708f48411.png)

All Tasks Exported to PDF:
![TasksPdfExport](https://user-images.githubusercontent.com/116143087/233644316-9a782326-ab13-4dd2-a511-88ef05c8f706.png)

All Tasks Exported to Excel:
![ExcelExport](https://user-images.githubusercontent.com/116143087/233644359-a00ac6de-bb27-4b44-8d07-4e33e16f26a2.png)
