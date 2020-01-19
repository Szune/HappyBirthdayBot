# HappyBirthdayBot
A telegram bot to wish humans happy birthdays.

Usernames cannot contain spaces and are supposed to be the Telegram username of the person.

#### Commands
| Command | Description | Admin only |
| ------- | ----------- | ----- |
| /birthcommands | Lists commands | |
| /birthlist | Lists birthdays | |
| /birthadd | Add birthday | if adding a user other than yourself | 
| /birthdelete | Delete birthday | ✔ | 
| /birthquit | Quit BirthdayBot | ✔ |

#### Adding/deleting admins
Enter ```admin add <username>``` or ```admin delete <username>``` in the console.

Alternatively edit the config.json file directly.
##### Adding admin in config.json
Change the following:
```"Admins": []```

To: ```"Admins": ["theUserYouWantToBeAdmin"]```

There can be multiple admins as well, just separate them with commas, e.g.

```"Admins": ["admin1", "admin2"]```

#### Changing time zone
Change ```TimeZoneId``` in config.json

```TimeZoneId```s can be enumerated or written to a file by entering ```timezone list``` or ```timezone file``` in the console.

#### Setting up the bot
Edit the ```Token``` and ```ChatId``` properties in config.json to your bot token and the chat id of the chat you wish to use the bot in.