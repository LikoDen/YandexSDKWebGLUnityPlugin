mergeInto(LibraryManager.library, {

ShowFullscreenAd: function() {
showFullScreenAdv();
},

ShowRewardedAd: function(placement) {
showRewardedAdv(placement);
return placement;
},
OpenRateUs: function(placement){
    openRateUs();
},
Debug: function()
{
console.log("I work");
}
});
