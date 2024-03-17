import aiohttp
import asyncio 

class HttpClient:
    def __init__(self, root: str) -> None:
        self.root = root 

    

    async def request(self, method: str, endpoint: str, *args, **kwargs):
        async with aiohttp.ClientSession(headers={
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
        }) as session:
            async with session.request(method, self.root + endpoint, **kwargs) as resp:
                if resp.ok:
                    if "application/json" in resp.headers.get("content-type", ""):
                        return (await resp.json())
                else:
                    return {}