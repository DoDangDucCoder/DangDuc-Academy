import requests
import json
import time

BASE_URL = "http://localhost:5194/api/v1/youtube-import"

channels = [
    "https://www.youtube.com/@anonystick/playlists",
    "https://www.youtube.com/@F8VNOfficial/playlists"
]

def preview_channel(url):
    print(f"Previewing channel: {url}")
    res = requests.post(f"{BASE_URL}/preview-channel", json={"ChannelUrl": url})
    if res.status_code == 200:
        data = res.json()
        playlists = data.get("data", [])
        print(f"Found {len(playlists)} playlists.")
        return playlists
    else:
        print(f"Failed to preview channel: {res.status_code} {res.text}")
        return []

def import_playlist(playlist_id):
    print(f"Importing playlist: {playlist_id}")
    req = {
        "PlaylistUrlOrId": playlist_id,
        "InstructorId": 1,
        "PublishNow": True
    }
    res = requests.post(f"{BASE_URL}/import-playlist", json=req)
    if res.status_code == 201:
        print(f"Successfully imported: {playlist_id}")
    else:
        print(f"Failed to import {playlist_id}: {res.status_code} {res.text}")

def main():
    # Wait for backend to be ready
    for i in range(10):
        try:
            requests.get("http://localhost:5194/health/live", timeout=2)
            break
        except:
            time.sleep(2)

    for channel in channels:
        playlists = preview_channel(channel)
        for pl in playlists:
            pid = pl.get("id")
            if pid:
                import_playlist(pid)

if __name__ == "__main__":
    main()
