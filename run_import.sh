#!/bin/sh
curl -X POST "http://localhost:5194/api/v1/YoutubeImport/preview-channel" \
     -H "Content-Type: application/json" \
     -d '{"ChannelUrl": "https://www.youtube.com/@anonystick/playlists"}'
