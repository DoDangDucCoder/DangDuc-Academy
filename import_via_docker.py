import subprocess
import json

channels = [
    "https://www.youtube.com/@anonystick/playlists",
    "https://www.youtube.com/@F8VNOfficial/playlists"
]

def run_curl(url, data):
    cmd = [
        "docker", "exec", "-i", "hdnx-backend", "curl", "-s", "-X", "POST",
        f"http://localhost:5194/api/v1/youtube-import/{url}",
        "-H", "Content-Type: application/json",
        "-d", json.dumps(data)
    ]
    res = subprocess.run(cmd, capture_output=True, text=True, encoding="utf-8")
    if res.returncode == 0:
        return res.stdout
    else:
        print("Error:", res.stderr)
        return None

for channel in channels:
    print(f"Previewing {channel}...")
    out = run_curl("preview-channel", {"ChannelUrl": channel})
    if out:
        try:
            data = json.loads(out)
            playlists = data.get("data", [])
            print(f"Found {len(playlists)} playlists in {channel}.")
            for pl in playlists:
                pid = pl.get("playlistId")
                if pid:
                    print(f"Importing {pid}...")
                    import_out = run_curl("import-playlist", {
                        "PlaylistUrlOrId": pid,
                        "InstructorId": 1,
                        "IdAuthor": 1,
                        "IdCategory": 1,
                        "PublishNow": True
                    })
                    print("Result:", import_out)
        except Exception as e:
            print("Error parsing json:", e)
            print("Output was:", out)
