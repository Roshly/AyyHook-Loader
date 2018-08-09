# Ayyhook | Open Source C# Cheat Loader

If you need support ask people on [Discord](https://ayyware.com/ayyhook)!
My discord: Roshly#0112

By downloading and using the source you agree to the [License](#license) that comes with the loader.

### Sponsored by [Ember Servers](https://emberservers.net)!
Use promo code "ROSHLY" for 10% off your first month!

## What's New?

- New Mybb authentication (Made by Thaisen)
- New design
- You can FINALLY press enter in a text box (when logging in)
- Easier to setup
- A youtube video on how to finally set it up!

## Setup Video:

- https://www.youtube.com/watch?v=nHAMYdqHGbA

## Screenshots

<p align="center">
 <img src="https://i.gyazo.com/f37574ab0c1cde57c5af438177611504.png">
</p>

<p align="center">
 <img src="https://i.gyazo.com/ad435cb81b6b4b188611c7d59abb6bf3.png">
</p>

## SQL Setup For HWID

1. Enter your PHPMyAdmin (Or whatever tool you use for SQL management) and navigate to your mybb_users.

2. Click on the "Structure" tab at the top of PHPMyAdmin.

3. Now add a new column named `hwid` that is a `varchar` with a max limit of `255`.

## Web Files Setup

1. Upload the check.php.

2. Change the login information to yours in lines 2-3 and that's all!


### Anti-Leak Instructions

Removed becuase too many idiots dont know what their doing and this is for pasters so whats the point of preventing your indigo paste from being stolen when the source is everywhere!

## Loader Form Files

1. Change all of the links in settings.cs to match your website.

2. Test and attempt to fix errors before spamming me.

## (Optional) Steps on how to install Costura Fody:
1. Right click on project and click on manage NuGet packages
2. Click on the browse tab
3. Search up Costura.Fody
4. Press install!

## License

This repo is listed with a [MIT license](https://github.com/Roshly/AyyHook-Loader/blob/master/LICENSE) which allows this to be used for commercial use, personal use and distribution and allows for modification of the source BUT does NOT allow me to be liable for what you do with the source and does not offer any warranty.

## FAQ

**Q: I can't open the loader.**

A: Make sure all metro dll files are in the same location as the exe! or install Costura Fody (can be found by scrolling up)
___
**Q: CSGO crashes after injection or wont inject?**

A: Make sure dll's are not protected by vmprotect! VMPROTECT IS USELESS! Vmprotect does change the signature when putting it in sigbench but it is just getting sig of dll but when you inject vmprotect doesnt protect anything but instead gets removed so your original dll sig (before you vmprotected it) is what is being scanned! (Note: This is only confirmed for old version of vmprotect v2.x.x)
___
**Q: Getting a error such as this? https://i.imgur.com/YOVCxdr.png**

A: This is most commonly due to error connecting to server and has NOTHING to do with your incorrect login. This issue is most commonly found when using blazing fast or some kind of anti ddos solution (Tested that it does work with cloudflare). A simple solution is to either to connect to the main server ip or buy hosting from Ember Servers!
___
**Q: Login incorrect!**

A: Read Sql login is incorrect below to fix.
___
**Q: Sql login is incorrect?**

A: This is one of those common issues mainly for people using cpanel which auto generates your password.
In order to fix it, simply make sure that your password does NOT contrain any symbols!
___
**Q: Is this a cheat for Counter Strike?**

A: No, this is a tool for being able to sell cheats without giving your .dll file to your users
___
**Q: Is this only for Counter Strike?**

A: No! Although it is targeted towards CS:GO it can literally be used for any game that takes dll based cheats
___
**Q: Is this detected by VAC?**

A: At the time of writing no. But make sure you change the signature of the loader to some extent.
___
**Q: If I am using this, do I have to give you credit?**

A: [The license for the project](https://github.com/Roshly/AyyHook-Loader/blob/master/LICENSE)
___
**Q: Can I use this for a massive P2C?**

A: Yes, but your stuff WILL get leaked eventually. I'd recommend using this for a private cheat for your friends with a max of like 30 members.
___
**Q: Do I need a website?**

A: Yes and no. You can make it local only by using a tool such as XAMPP but if you want it to be available for others to use you should get a website. Port forwarding would work too but I advise against it.
___
**Q: How do I make paid usergroups on MyBB?**

A: https://community.mybb.com/thread-123597.html ALSO if you wanna be a real meme and need invite codes: https://community.mybb.com/thread-113141.html

## To Do List:
- Add dll encryption
- Fix grammar issues in code
- Add more comments to other files
- Also delete the folder after injection

## Credits

[HazzardEdit](https://www.youtube.com/channel/UCG0LukbgMa6vJkA_JJ4Jepg) for the [HWID creation and encryption code.](https://www.youtube.com/watch?v=M1-pAqPqJcw)

[weakspider](https://www.unknowncheats.me/forum/members/172964.html) for the [Injection method](https://www.unknowncheats.me/forum/c-/213037-x86-manual-map-injection.html)

[Thaisen](https://github.com/ThaisenPM): Helped fix injection method, Made the web handler & let me straight up c + p his readme file :D. Also the dev of nova hook!
