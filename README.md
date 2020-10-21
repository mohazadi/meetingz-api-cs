#### [Meetingz.Ir](https://my.meetingz.ir/) ⁖ One Of The Best Web Conferencing Platform [*](#bigbluebutton)

پلتفرم آموزش آنلاین  میتینگـــز
================
برگزاری کلاس آنلاین بدون نیاز به نصب
---------------
قابلیت ضبط با ربات هوشمند
---------------
لیست جلسات برگزار شده، افراد حاضر یا غایب در کلاس، مدت زمان حضور در کلاس
---------------
آمار لحظه به لحظه و آنلاین کل تعداد شرکت‌کنندگان
---------------
توزیع هوشمند جلسات بر روی فضای ابری
---------------
---------------

This document describes the Meetingz application programming
interface (API).

For developers, this API enables you to

-   create meetings
-   join meetings
-   end meetings
-   get recordings for past meetings (and delete them)
-   upload closed caption files for meetings

To make an API call to your Meetingz server, your application makes
HTTPS requests to the Meetingz server API endpoint (usually the
server’s hostname followed by `https://api.meetingz.ir/<youe_key>/`). All API calls must include checksum computed with
a secret shared with the Meetingz server.

The Meetingz server returns an XML response to all API calls.

Updates to API in Meetingz
--------------------------------------------------------------------------------------------------------------------------------------------------------------

Updated in 1.0.0:

-   **CreateMeeting**
-   **JoinMeeting**
-   **GetMeetings** 
-   **GetMeetingInfo** 
-   **IsMeetingRunning** 
-   **EndMeeting** 

In the feature:
-   **GetRecordings**
-   **UpdateRecordings**


API Security Model
=======================================================================================================================

The Meetingz API security model enables 3rd-party applications to
make API calls (if they have the shared secret), but not allow other
people (end users) to make API calls.

The Meetingz API calls are almost all made server-to-server. The web
application makes HTTPS requests to the Meetingz server’s API end
point.

You can retrieve your Meetingz API parameters (API endpoint and
shared secret) registering on [my.meetingz.ir](https:://my.meetingz.ir)

Here’s a sample return

``` {.highlight}
    URL: https://api.meetingz.ir/test/
    Secret: 9184e6b9-e778-45f5-9c69-d0dfb6246264
```

You should *not* embed the shared secret within a web page and make
Meetingz API calls within JavaScript running within a browser. The
built-in debugging tools for modern browser would make this secret
easily accessibile to any user. Once someone has the shared secret for
your Meetingz server, they could create their own API calls. The
shared secret should only be accessibile to the server-side components
of your application (and thus not visible to end users).
 
Usage
--------------------------------------------------------------------------------

To use the security model, you must be able to create an SHA-1 checksum
out of the call name *plus* the query string *plus* the shared secret
that you configured on your server. To do so, follow these steps:

1.  Create the entire query string for your API call without the
    checksum parameter.
    -   Example for create meeting API call:
        `name=Test+Meeting&meetingID=abc123&attendeePW=111222&moderatorPW=333444`

2.  Prepend the API call name to your string
    -   Example for above query string:
        -   API call name is `create`
        -   String becomes:
            `createname=Test+Meeting&meetingID=abc123&attendeePW=111222&moderatorPW=333444`

3.  Now, append the shared secret to your string
    -   Example for above query string:
        -   shared secret is
            `639259d4-9dd8-4b25-bf01-95f9567eaf4b`
        -   String becomes:
            `createname=Test+Meeting&meetingID=abc123&attendeePW=111222&moderatorPW=333444639259d4-9dd8-4b25-bf01-95f9567eaf4b`

4.  Now, find the SHA-1 sum for that string (implementation varies based
    on programming language).
    -   the SHA-1 sum for the above string is:
        `1fcbb0c4fc1f039f73aa6d697d2db9ba7f803f17`

5.  Add a checksum parameter to your query string that contains this
    checksum.
    -   Above example becomes:
        `name=Test+Meeting&meetingID=abc123&attendeePW=111222&moderatorPW=333444&checksum=1fcbb0c4fc1f039f73aa6d697d2db9ba7f803f17`

You **MUST** send this checksum with **EVERY** API call. Since end users
do not know your shared secret, they can not fake calls to the server,
and they can not modify any API calls since changing a single parameter
name or value by only one character will completely change the checksum
required to validate the call.

 
API Resources
========================================================================================================

Administration
-----------------------------------------------------------------------------------------------------------

The following section describes the administration calls

-   **create:**                Creates a new meeting.
-   **join:**                  Join a new user to an existing meeting.
-   **end:**                   Ends meeting.

Monitoring
-----------------------------------------------------------------------------------------------

The following section describes the monitoring calls

-   **isMeetingRunning:**   Checks whether if a specified meeting is running.
-   **getMeetings:**        Get the list of Meetings.
-   **getMeetingInfo:**     Get the details of a Meeting.

Recording
--------------------------------------------------------------------------------------------

-   **getRecordings:**            Get a list of recordings.
-   **deleteRecordings:**         Deletes an existing recording
-   **updateRecordings:**         Updates metadata in a recording.

API Calls
============================================================================================

The following response parameters are standard to every call and may be
returned from any call.

**Parameters:**

-   **checksum:**   This is basically a SHA-1 hash of `callName + queryString + sharedSecret`. 
    The security salt will be configured into the application at deploy time. All calls to the API must include the checksum parameter.
  ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

**Response:**

-   **returncode:** Indicates whether the intended function was successful or not. Always one of two values:\
                                        `FAILED` – There was an error of some sort – look for the message and messageKey for more information. Note that if the `returncode` is FAILED, the call-specific response parameters marked as “always returned” will not be returned. They are only returned as part of successful responses.\
                                        `SUCCESS` – The call succeeded – the other parameters that are normally associated with this call will be returned.
-   **message:**      A message that gives additional information about the status of the call. A message parameter will always be returned if the returncode was `FAILED`. A message may also be returned in some cases where returncode was `SUCCESS` if additional information would be helpful.
-   **messageKey:**   Provides similar functionality to the message and follows the same rules. However, a message key will be much shorter and will generally remain the same for the life of the API whereas a message may change over time. If your third party application would like to internationalize or otherwise change the standard messages returned, you can look up your own custom messages based on this messageKey.
  ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

create
-----------------------------------------------------------------------------------

Creates a meeting.

The create call is idempotent: you can call it multiple times with the
same parameters without side effects. This simplifies the logic for
joining a user into a session as your application can always call create
before returning the join URL to the user. This way, regardless of the
order in which users join, the meeting will always exist when the user
tries to join (the first `create` call actually creates the meeting; subsequent calls
to `create` simply return
`SUCCESS`).

The Meetingz server will automatically remove empty meetings that
were created but have never had any users after a number of minutes
specified by `meetingExpireIfNoUserJoinedInMinutes` defined in server.

**Resource URL:**

https://api.meetingz.ir/<youe_key>/api/create?[parameters]&checksum=[checksum]

**Parameters:**

-   **name:**                             A name for the meeting.
-   **meetingID:**                        A meeting ID that can be used to identify this meeting by the 3rd-party application. \
                                    This must be unique to the server that you are calling: different active meetings can not have the same meeting ID. \
                                    If you supply a non-unique meeting ID (a meeting is already in progress with the same meeting ID), then if the other parameters in the create call are identical, the create call will succeed (but will receive a warning message in the response). The create call is idempotent: calling multiple times does not have any side effect. This enables a 3rd-party applications to avoid checking if the meeting is running and always call create before joining each user.\
                                    Meeting IDs should only contain upper/lower ASCII letters, numbers, dashes, or underscores. A good choice for the meeting ID is to generate a [GUID](http://en.wikipedia.org/wiki/Globally_unique_identifier) value as this all but guarantees that different meetings will not have the same meetingID.
-   **attendeePW:**                       The password that the  [join](#join) URL can later provide as its `password` parameter to indicate the user will join as a viewer. If no `attendeePW` is provided, the `create` call will return a randomly generated `attendeePW` password for the meeting.
-   **moderatorPW:**                      The password that will [join](#join) URL can later provide as its `password` parameter to indicate the user will as a moderator. if no `moderatorPW` is provided, `create` will return a randomly generated `moderatorPW` password for the meeting.
-   **welcome:**                          A welcome message that gets displayed on the chat window when the participant joins. You can include keywords (`%%CONFNAME%%`, `%%DIALNUM%%`, `%%CONFNUM%%`) which will be substituted automatically.\
                                    This parameter overrides the default `defaultWelcomeMessage` in `server.meetingz.properties`.\
                                    The welcome message has limited support for HTML formatting. Be careful about copy/pasted HTML from e.g. MS Word, as it can easily exceed the maximum supported URL length when used on a GET request.
-   **maxParticipants:**                  Set the maximum number of users allowed to joined the conference at the same time.
-   **logoutURL:**                        The URL that the Meetingz client will go to after users click the OK button on the ‘You have been logged out message’. This overrides the value for `web.logoutURL` in   `server.meetingz.properties`.
-   **record:**                           Setting ‘record=true’ instructs the Meetingz server to record the media and events in the session for later playback. The default is false.\
                                    In order for a playback file to be generated, a moderator must click the Start/Stop Recording button at least once during the sesssion; otherwise, in the absence of any recording marks, the record and playback scripts will not generate a playback file. See also the `autoStartRecording` and `allowStartStopRecording` parameters in server.
-   **duration:**                         The maximum length (in minutes) for the meeting.\
                                    Normally, the Meetingz server will end the meeting when either (a) the last person leaves (it takes a minute or two for the server to clear the meeting from memory) or when the server receives an [end](#end) API request with the associated meetingID (everyone is kicked and the meeting is immediately cleared from memory).\
                                    Meetingz begins tracking the length of a meeting when it is created. If duration contains a non-zero value, then when the length of the meeting exceeds the duration value the server will immediately end the meeting (equivalent to receiving an end API request at that moment).
-   **meta:**                              This is a special parameter type (there is no parameter named just `meta`).\
                                    You can pass one or more metadata values when creating a meeting. These will be stored by Meetingz can be retrieved later via the getMeetingInfo and getRecordings calls.\
                                    Examples of the use of the meta parameters are `meta_Presenter=Jane%20Doe`, `meta_category=FINANCE`, and `meta_TERM=Fall2016`.
-   **moderatorOnlyMessage:**              Display a message to all moderators in the public chat.\
                                    The value is interpreted in the same way as the `welcome` parameter.
-   **autoStartRecording:**                Whether to automatically start recording when first user joins (default `false`).\
                                    When this parameter is `true`, the recording UI in Meetingz will be initially active. Moderators in the session can still pause and restart recording using the UI control.\<br/\
                                    NOTE: Don’t pass `autoStartRecording=false` and `allowStartStopRecording=false` - the moderator won’t be able to start recording!
-   **allowStartStopRecording:**           Allow the user to start/stop recording. (default true)\
                                    If you set both `allowStartStopRecording=false` and `autoStartRecording=true`, then the entire length of the session will be recorded, and the moderators in the session will not be able to pause/resume the recording.
-   **webcamsOnlyForModerator:**           Setting `webcamsOnlyForModerator=true` will cause all webcams shared by viewers during this meeting to only appear for moderators (added 1.1)
-   **muteOnStart:**                       Setting `muteOnStart=true` will mute all users when the meeting starts. (added 2.0)
-   **allowModsToUnmuteUsers:**            Default `allowModsToUnmuteUsers=false`. Setting to `allowModsToUnmuteUsers=true` will allow moderators to unmute other users in the meeting. (added 2.2)
-   **lockSettingsDisableCam:**            Default `lockSettingsDisableCam=false`. Setting `lockSettingsDisableCam=true` will prevent users from sharing their camera in the meeting. (added 2.2)
-   **lockSettingsDisableMic:**            Default `lockSettingsDisableMic=false`. Setting to `lockSettingsDisableMic=true` will only allow user to join listen only. (added 2.2)
-   **lockSettingsDisablePrivateChat:**    Default `lockSettingsDisablePrivateChat=false`. Setting to `lockSettingsDisablePrivateChat=true` will disable private chats in the meeting. (added 2.2)
-   **lockSettingsDisablePublicChat:**     Default `lockSettingsDisablePublicChat=false`. Setting to `lockSettingsDisablePublicChat=true` will disable public chat in the meeting. (added 2.2)
-   **lockSettingsDisableNote:**           Default `lockSettingsDisableNote=false`. Setting to `lockSettingsDisableNote=true` will disable notes in the meeting. (added 2.2)
-   **lockSettingsLockedLayout:**          Default `lockSettingsLockedLayout=false`. Setting to `lockSettingsLockedLayout=true` will lock the layout in the meeting. (added 2.2)
-   **lockSettingsLockOnJoin:**            Default `lockSettingsLockOnJoin=true`. Setting to `lockSettingsLockOnJoin=false` will not apply lock setting to users when they join. (added 2.2)
  ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

  
**Example Requests:**

-   https://api.meetingz.ir/<youe_key>/api/create?name=Test&meetingID=test01&checksum=1234
-   https://api.meetingz.ir/<youe_key>/api/create?name=Test&meetingID=test01&moderatorPW=mp&attendeePW=ap&checksum=wxyz
-   https://api.meetingz.ir/<youe_key>/api/create?name=Test&meetingID=test01&moderatorPW=mp&attendeePW=ap&meta\_presenter=joe&meta\_category=education&checksum=abcd


join
-----------------------------------------------------------------------------

Joins a user to the meeting specified in the meetingID parameter.

**Resource URL:**

https://api.meetingz.ir/<youe_key>/api/join?[parameters]&checksum=[checksum]

**Parameters:**

-   **fullName:**             The full name that is to be used to identify this user to other conference attendees.
-   **meetingID:**            The meeting ID that identifies the meeting you are attempting to join.
-   **password:**             The password that this attendee is using. If the moderator password is supplied, he will be given moderator status (and the same for attendee password, etc)
-   **userID:**               An identifier for this user that will help your application to identify which person this is. This user ID will be returned for this user in the getMeetingInfo API call so that you can check

**Example Requests:**

-   https://api.meetingz.ir/<youe_key>/api/join?meetingID=test01&password=mp&fullName=John&checksum=1234
-   https://api.meetingz.ir/<youe_key>/api/join?meetingID=test01&password=ap&fullName=Mark&checksum=wxyz
-   https://api.meetingz.ir/<youe_key>/api/join?meetingID=test01&password=ap&fullName=Chris&createTime=273648&checksum=abcd


isMeetingRunning
-----------------------------------------------------------------------------------------------------------------

This call enables you to simply check on whether or not a meeting is
running by looking it up with your meeting ID.

**Resource URL:**

https://api.meetingz.ir/<youe_key>/api/isMeetingRunning?[parameters]&checksum=[checksum]

**Parameters:**

-   **meetingID:**         The meeting ID that identifies the meeting you are attempting to check on.


end
--------------------------------------------------------------------------

Use this to forcibly end a meeting and kick all participants out of the
meeting.

**Resource URL:**

-   https://api.meetingz.ir/<youe_key>/api/end?[parameters]&checksum=[checksum]

**Parameters:**

-   **meetingID:**        The meeting ID that identifies the meeting you are attempting to end.
-   **password:**         The moderator password for this meeting. You can not end a meeting using the attendee password.

getRecordings
--------------------------------------------------------------------------------------------------------

Retrieves the recordings that are available for playback for a given
meetingID (or set of meeting IDs).

**Resource URL:**

https://api.meetingz.ir/<youe_key>/api/getRecordings?[parameters]&checksum=[checksum]

**Parameters:**

-   **meetingID:**        A meeting ID for get the recordings. It can be a set of meetingIDs separate by commas. If the meeting ID is not specified, it will get ALL the recordings. If a recordID is specified, the meetingID is ignored.
-   **recordID:**         A record ID for get the recordings. It can be a set of recordIDs separate by commas. If the record ID is not specified, it will use meeting ID as the main criteria. If neither the meeting ID is specified, it will get ALL the recordings. The recordID can also be used as a wildcard by including only the first characters in the string.
-   **state:**            Since version 1.0 the recording has an attribute that shows a state that Indicates if the recording is [processing|processed|published|unpublished|deleted]. The parameter state can be used to filter results. It can be a set of states separate by commas. If it is not specified only the states [published|unpublished] are considered (same as in previous versions). If it is specified as “any”, recordings in all states are included.
-   **meta:**             You can pass one or more metadata values to filter the recordings returned. The format of these parameters is the same as the metadata passed to the `create` call.

**Example Requests:**

-   https://api.meetingz.ir/<youe_key>/api/getRecordings?checksum=1234
-   https://api.meetingz.ir/<youe_key>/api/getRecordings?meetingID=CS101&checksum=abcd
-   https://api.meetingz.ir/<youe_key>/api/getRecordings?meetingID=CS101,CS102&checksum=wxyz
-   https://api.meetingz.ir/<youe_key>/api/getRecordings?recordID=652c9eb4c07ad49283554c76301d68770326bd93-1462283509434&checksum=wxyz
-   https://api.meetingz.ir/<youe_key>/api/getRecordings?recordID=652c9eb4c07ad49283554c76301d68770326bd93-1462283509434,9e359d17635e163c4388281567601d7fecf29df8-1461882579628&checksum=wxyz
-   https://api.meetingz.ir/<youe_key>/api/getRecordings?recordID=652c9eb4c07ad49283554c76301d68770326bd93&checksum=wxyz
-   https://api.meetingz.ir/<youe_key>/api/getRecordings?recordID=652c9eb4c07ad49283554c76301d68770326bd93,9e359d17635e163c4388281567601d7fecf29df8&checksum=wxyz

[*](#bigbluebutton)