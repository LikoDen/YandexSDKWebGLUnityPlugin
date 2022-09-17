mergeInto(LibraryManager.library, {

ShowFullscreenAd: function() {
showFullScreenAdv();
},

ShowRewardedAd: function(placement) {
showRewardedAdv(placement);
return placement;
},

Debug: function()
{
console.log("I work");
}
});