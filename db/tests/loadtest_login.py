
import time
import enum
import asyncio
import math
import aiohttp
import subprocess
import os

def millis():
    return int(round(time.time() * 1000))

async def sendRequestsConcurrently(load):
    tasks = []
    sem = asyncio.Semaphore(1000)
    testStart = millis()
    
    http = aiohttp.ClientSession()
    for i in range(load):
        #await asyncio.sleep(0.01)
        task = asyncio.ensure_future(loginTask(sem, http))
        tasks.append(task)
    
    success = 0
    failure = 0
    responses = await asyncio.gather(*tasks)
    for i in responses:
        if i == 1:
            success += 1
        else:
            failure += 1
    await http.close()
    print("done")
    print("success: "+str(success))
    print("failure: "+str(failure))
    print(f"time to complete {load} requests: {millis()-testStart}ms")

async def loginTask(sem, http):
    async with sem:
        return await login(http)

async def login(http):
    url = "https://15paucwkia.execute-api.us-east-1.amazonaws.com/dev/api/authenticate"
    headers = {
        "Content-Type": "application/json",
    }
    payload = {
        "username": "doneale46",
        "password": "exist123"
    }
    print(f"[login]: sending request")
    start = millis()
    
    async with http.post(url, json=payload, headers=headers) as res:
        body = await res.json()
        now = millis()
        print(f"[login]: [{now-start}ms] result: "+str(body["statusCode"]))
        if (body["statusCode"] == 502):
            return -1
        else:
            return 1


asyncio.run(sendRequestsConcurrently(100))