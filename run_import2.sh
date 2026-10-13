#!/bin/sh
curl -v -X POST "http://localhost:5194/api/v1/youtube-import/preview-channel" \
     -H "Content-Type: application/json" \
     -d '{"ChannelUrl": "https://www.youtube.com/@anonystick/playlists"}'
