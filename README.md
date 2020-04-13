(Note from Allen aka bobasaurus)
I forked this code from https://github.com/SDRplay/examples with the purpose of fixing the outdated C example and adding C# support for the v3 API.  

The sdrplay_api_v3_example.c file is the latest API v3 example code copied from SDRplay_API_Specification_v3.06.pdf, this is more up-to-date than the original C example code in sdrplay_api_example.c.  

I have also made a C# version of the API (using P/Invoke DllImport).  The C# API and example code are in the project "sdrplay_api_v3_csharp_console_example".  It copies the API v3 dll from the API's program files installation directory as a post build event.  



(Original readme text)
This repository has some example code to show what can be done with
the RSP. No warranty is given with any of this code and it is
provided as is.

play_sdr has been updated to use the new API functions and added support
for RSP1 and RSP2. Also it now supports both 8 and 16 bit output.

sdrplay_api_example.c has been added as an example - it's for the new
service based API (3.0)

If you would like to make contributions to this area, please send an
email to software@sdrplay.com
