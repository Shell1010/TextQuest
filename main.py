from textquest import TextQuest
import json 


config = json.loads(open("config.json").read())

TextQuest(config['username'], config['password']).start()