# Trivia Game Bot for Discord

Simple film themed trivia game bot made for discord, configured for personal discord server.

## Description

Adding this bot to a Discord server allows players to start trivia games focused on movies. Each game features film-related images and questions. Game can only be started and controlled by discord user who has specific role called "Gamemaster". Once a game begins, players can join, and only those who join will participate. The game runs in multiple rounds and keeps track of all participants. At the end of each round, points are awarded to the player who answers correctly first. When the game concludes, the bot displays the total scores of all participants. 

### Dependencies

* Discord
* .NET SDK (e.g., .NET 6 or later)

### Executing program

* Launch downloaded project
* If bot was launched succesfully it will appear as online in discord server

## Help

If users in trivia channel types:
```
?help
```
then list of commands will show up:
```
List of commands
Help - shows list of commands
Start - sends embeded messege that lets users to join the game in given time (number + s, m or h).
Picture - shows random picture so players can guess what it is. Type '--stop' to end guessing.
Question - shows random question so player can try to answer it. Type '--stop' to end guessing.
End - ends the current game and shows results.
Score - shows a leaderboard of all players that ever played. Sorted list requires extra option for sorting: 'Score' - sort by score, 'GamesPlayed' - sort by games, 'Username' - sort by username.
---------------------------------------------------------------------------
Help and score can only be used when game is not happening.
Commands for game in order: start, question/picture, end.
```
This should clarify how to run the bot.