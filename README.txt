StreamMosaic

This is a simple frontend to the free 'Livestreamer' utility. You can use it to connect to one or more currently running live streams via Livestreamer and get a local ip/port combination that you can use as a source in a tool like OBS. This lets you restream other people's content with annotations, or combine multiple source streams into one output stream.

You will want to install Livestreamer using the setup located at https://github.com/chrippa/livestreamer/releases (named something like 'livestreamer-x.x.x-win32-setup.exe') so that StreamMosaic can find it.

You can compile this with any modern version of Visual Studio (I use 2010); express should be fine, just make sure it can handle C#. If you're feeling too lazy to compile it, I usually put a build at https://dl.dropboxusercontent.com/u/1643240/StreamMosaic.exe .