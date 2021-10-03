# BaseKits

Customizable TShock Plugin to allow players to get kits with commands.  
SSC (Serve-side characters) must be enabled for this plugin to work properly.  

/startend *(Starts or ends a game, which is setting PvP to always)*  
/allowgetkits *(Allows or disallows players to get kits)*  
/kit \<Name> *(Gets a kit)*  

To customize the kits, create a `kits.json` file on the `tshock` folder, structured like this:

```
{
  "1": {
    "items": "1 2 3"
  },
  "2": {
    "items": "67 33 3",
    "life": "200"
  },
  "3": {
    "items": "90",
    "life": "360",
    "mana": "60"
  },
  "warrior": {
    "items": "300",
    "life": "400"
  }
}
```

The numbers in `items` are Item IDs separated by spaces.  
If no life is defined, it will be set to 100, if no mana is defined, it will be set to 20.
