# Description

I wrote this WPF application for my wife - she needs a tasks management application for her job.
However, everybody is free to use it.

The idea is simple: You can add, edit and delete tasks. Behind the scenes, I implemented editing by deleting and adding.
Every task has the following hardcoded fields (according to my wife's request): 
Mandatory - Subject, Frequency, Final Date, Desk, Status.
Optional - Intermediate Date, Related To, Description.

The frequency may be:
For recurring tasks - Every Week, Every Month, or Every Year
For non-recurring tasks - Once

There is a timer that checks which tasks are due today (either according to the intermediate date or according to the final date). If the current time is 9 o'clock or later, a notification is shown (9 o'clock is hardcoded according to my wife's request). When the user clicks the notification, the corresponding task is shown.

# Technology Stack
- As stated above, I used WPF.
- I used CommunityToolkit.Mvvm to adhere to the MVVM pattern. In some places, I broke to rules of MVVM to simplify the rules - I am not a purist.
- I used an Sqlite database to store the tasks. I think that in a desktop application without a server, Sqlite is a good fit.
- I used EF Core 6 for database access.
