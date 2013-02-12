#gooddata-csharp

There are two basic classes

1. SSOProvider.cs
2. ApiWrapper.cs

##SSOProvider
####PGP security:
1. By default uses http://www.bouncycastle.org/csharp/ for PGP encryption and does not require to install any additional software.
2. Can be used with gpg4win (Need to install from http://www.gpg4win.org/). Please see GoodData.Security.GPG namespace for more details.

##ApiWrapper
Wraps most of the GoodData REST API.

Most of the work was done by https://github.com/jkind/gooddata-csharp/network.
