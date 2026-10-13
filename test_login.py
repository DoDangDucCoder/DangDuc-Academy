import requests

url = "http://localhost:5194/api/v1/Authentication/login"
data = {"Email": "admin@gmail.com", "Password": "change_me"}
r = requests.post(url, json=data)
print("change_me:", r.status_code, r.text)

data = {"Email": "admin@gmail.com", "Password": "CHANGE_ME_USE_ENV"}
r = requests.post(url, json=data)
print("CHANGE_ME_USE_ENV:", r.status_code, r.text)

data = {"Email": "admin@gmail.com", "Password": "123"}
r = requests.post(url, json=data)
print("123:", r.status_code, r.text)
