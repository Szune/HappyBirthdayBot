# HappyBirthdayBot
A telegram bot to wish humans happy birthdays.

#### Commands
| Command | Description | Admin only |
| ------- | ----------- | ----- |
| /birthcommands | Lists commands | |
| /birthlist | Lists birthdays | |
| /birthadd | Add birthday | ✔ | 
| /birthdelete | Delete birthday | ✔ | 
| /birthquit | Quit BirthdayBot | ✔ |

#### Adding/deleting admins
Type 'admin add *username*', 'admin delete *username*' or edit the config.json file directly.

#### Changing time zone
Change "TimeZoneId" in config.json

#### Setting up the bot
Edit the "Token" and "ChatId" properties in config.json to your bot token and the chat id of the chat you wish to use the bot in.