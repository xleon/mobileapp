# Intercepting HTTP Requests

It can be useful to intercept HTTP requests between our app and the server when debugging issues with login, sign-up, or syncing. There are two useful tools which can be used for this purpose (there are definitely more of them, but these two were tested):
- [Charles](https://www.charlesproxy.com/) and [Charles for iOS](https://www.charlesproxy.com/documentation/ios/)
- [Proxyman](https://proxyman.io/) - recommended, but macOS only

## Instructions For Proxyman

- Download and install Proxyman on your Mac
- Install Proxyman Certificate on your Mac, simulator, and iOS or Android devices
    - in the Proxyman app, go to the `Certificate` menu item and follow the instructions
    - for more information, check the [Docs](https://docs.proxyman.io/debug-devices/macos)
- If you're debugging the app on your device:
    - make sure that the [Toggl staging certificate is installed on your device](../certificate.md)
    - make sure that your device and your computer are connected to the same Wi-Fi network
    - configure the proxy for your Wi-Fi network with your computer's IP address and proxy port
        - typically `192.168.1.2:9090`
        - the actual values will be shown when you click on the `Certificate > Install Certificate on iOS devices...` or `Certificate > Install Certificate on Android devices...` menu items
- Start recording network traffic with the "play" button
- You should now see requests to the `https://mobile.toggl.space` in Proxyman
    - _Tip: filter out requests to other domains to make your life easier."
- That's it :tada:
