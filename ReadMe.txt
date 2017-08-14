Run from DiscordBot3.0.exe
A chat bot/server admin assistant that plays music and manages channels. Discord has currently changed thier server gateway so it requires a rewite and has been deprecated
Bot Usage 
-Client Side
	- Three Different Different output windows
		- Bot Status - out puts our status and any shared info from servers
		- Server status - will out put all servers and info about them
		- Console window - outputs current relevent data and console commands
			Ex - The client command execute text box
	-Bot token - Is where you put your token for the bots avatar
		- Get one at discord developers site at apps
	-Music - Put the folder to use to get songs from - not in yet
	-Audio Capture Stream - Used to capture from and audio device - not in yet
		-Refresh - refreshes device list
		-Select Outout - Select out put stream
		-mute/unmute - turn on / turn off source capture
		-Record - Record the stream
	-Games To play - A list of games for her to be playing at randomly
		-text field - the name to add for a game to play
		-Add - actualy adds the game
		-Remove - Removes the selected index
	-Sound board Bindings - A list for trees of sounds for the sound board
		-Top List Box - Lists Catagories or trees for binding
			-Catagory Key Text Field - the key (the discord key word for tree) to bind to
			-Discriptor - optional message to display when listing to help with usage on discord server side
			-Add - adds catagory using the text fields
			-Note: Can't have duplicate keys
		-Bottom List Box - Lists the Sound Files Locations and Keys
			-Catagory to add to - The Catagory you wish to bind the sound to
			-Sound Key - a key to use to call the sound from discord
			-File Name - The name of the file in _SoundBoard
			-Note: There can only be on sound file name but shared keys
			if keys are shared they will be randomly selected between shared keys
		-How To : To Bind sound - Fist add sound file '../DiscordBot3.0/_SoundBoard_'. 
			After Make a Catagory to call from. 
			Then make a key bind sound to.

	 
-Discord Server Side
	-!is General comands
		-!help - gets a out out to discord to help nav with commands
		-!again - does the last command again (save typing)
		-There are different trees from here for commands
			-!~is for chat funcs
				-!~help for chat commands
			-!!is for utility funcs
				-!!help for utilitie Commands
			-!~is for Dj or sound funcs
				-!@help for DJ Commands
			-!#is for Admin funcs
				-!#help for admin Commands
