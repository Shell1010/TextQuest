import aiohttp
from smartfox import Smartfox
import random
import time
import asyncio
import json 

config = json.loads(open("config.json").read())
name = config['username']
passw = config['password']

root = "https://game.aq.com"
sfs = Smartfox("")
print(time.time())


async def login(username: str, password: str):
    headers = {
        "accept": "*/*",
        "accept-language": "en-US",
        "connection": "keep-alive",
        "artixmode": "launcher",
        "host": "game.aq.com",
        "content-type": "application/x-www-form-urlencoded",
        "sec-fetch-dest": "embed",
        "sec-fetch-mode": "no-cors",
        "sec-fetch-site": "same-origin",
        "user-agent": "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Safari/537.36",
        "x-requested-with": "ShockwaveFlash/32.0.0.371"
    }
    url = f"{root}/game/api/login/now?ran=0.{random.randint(1000000000000000, 9999999999999999)}"
    print(url)
    body = f"pass={password}&user={username}&option=1"
    # async with aiohttp.ClientSession() as session:
    #     async with session.post(url, headers=headers, data=body) as resp:
    #         print(resp.status)
    #         data = await resp.json()
    #         with open("test.json", "a+") as f:
    #             json.dump(data, f, indent=4)


asyncio.run(login(name, passw))