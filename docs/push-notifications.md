Push Notifications
==================

Our app uses push notifications to trigger sync when a change occurs on the server before the user syncs to the app. The idea is to keep time entries in sync at all times (as much as the mobile operating system allows us to).

It is important to keep in mind that it is possible that a notification is not delivered to the app by the device (Apple Push Notification service or Firebase Cloud Messaging) or the operating system does not call our app when it is in background (to preserve battery life). It therefore isn't possible for us to guarantee that the app will always be in sync with the server even if the user is online all the time and the server sends push notifications to our app frequently.

This document covers several important parts of the whole implementation:

1. What the apps do when a push notification is received
1. Firebase Console setup
1. FCM token lifecycle and communication with the API server
1. iOS specific documentation
1. Android specific documentation

## App Response To Push Notifications

The app reacts differently when a push notification is received depending on the state of the app:
- when the app is in the foreground and it is being used by the user, we pull just the time entries first to be able to update the currently running time entry as soon as possible, but then we also immediately execute a full sync
- when the app is in the background, we fetch just the time entries using.

We use Firebase Remote Config to control the rollout of the feature to the users. The config key is `handle_push_notifications`.

### "Sync Light"

The name _Sync Light_ is a nickname for a simplified lightweight sync workflow triggered by `ISyncManager.PullTimeEntries()`. It sends just 1 HTTP requests and it is intended to finish much faster than regular _Full Sync_ and so it could be used when the app is in background and we have a limited time to update the app. The workflow is described in detail in [the syncing docs](syncing/pull-time-entries.md).

## Firebase Console Setup

We use the `Toggl Mobile` project for push notification. This is the same project we use for analytics and performance measurement. The APNs auth key is uploaded to all of our profiles - Debug, AdHoc, and production. Other than that, there is nothing interesting about the Firebase setup.

## FCM Token Lifecycle

During push sync, we try to push an FCM token if one of these conditions is met:
- the FCM token hasn't been pushed to the server yet (e.g., right after the user logs in, or if the FCM token changes),
- it is more than 5 days since the token was pushed to the server.
If the HTTP request fails, we do not repeat it right away, but we will attempt the next time, because the first condition still hasn't been met.

If the FCM token changes, our app is notified by the operating system, and we attempt to push the new token to the server right away. If this request fails (e.g., the user is offline at the moment), we will ignore the failure and the token will be synced later when the app syncs.

When the user logs out of the app, we invalidate the currently valid FCM token to stop receiving further notifications. We also try to inform the server so it can remove the token from the DB but we don't repeat the request if it fails, because the token stored on the server is useless anyway.

We use Firebase Remote Config to control whether the tokens are pushed to the server or not. The configuration key is `register_push_notifications_token_with_server`.

The API V9 endpoints we use are `POST|DELETE api/v9/me/push_services`. The documentation for these endpoints can be found [in Notion](https://www.notion.so/API-endpoints-d8b2ed5a93d74d8893f1862eb57eb903).

## iOS Specific Documentation

_This section will be added in the future._

## Android Specific Documentation

_This section will be added in the future._
