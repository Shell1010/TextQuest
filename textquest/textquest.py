import random
import asyncio
from .api import HttpClient
from urllib.parse import urlencode

class TextQuest:
    def __init__(self, username: str, password: str) -> None:
        self.user = username
        self.passw = password
        self.http = HttpClient("https://game.aq.com")



    async def login(self, option: int = 0):
        body = {
            "pass": self.passw,
            "user": self.user,
            "option": option
        }
        return await self.http.request(
            "post",
            f"/game/api/login/now?ran=0.{random.randint(1000000000000000, 9999999999999999)}",
            data=urlencode(body)
        )
    
    async def refresh_servers(self):
        return await self.http.request(
            "get",
            f"/game/api/data/servers?v=0.{random.randint(1000000000000000, 9999999999999999)}"
        )
    
    def start(self):
        async def runner():
            data = await self.login()
            print(data)
        asyncio.run(runner())