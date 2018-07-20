document.write('\
<a href="http://cliversoft.com" title="CliverSoft.com">\
    <TABLE cellSpacing="1" cellPadding="0" border="0" style="align-self: center;">\
		<TR>\
			<TD bgcolor=#ff9966 width="5px" height="5px" id="copyright_td1"></TD>\
			<TD bgcolor=#999988 width="5px" height="5px"  id="copyright_td2"></TD>\
		</TR>\
		<TR>\
			<TD bgcolor=#ddddcc width="5px" height="5px"  id="copyright_td3"></TD>\
			<TD bgcolor=#bb99dd width="5px" height="5px"  id="copyright_td4"></TD>\
		</TR>\
	</TABLE>\
 </a>\
');

var copyright = {		
    get_random: function(max){
        var ran_unrounded = Math.random() * max;
        return Math.floor(ran_unrounded);
    },

    dec2hex: function(d) {
        var hD = "0123456789ABCDEF";
        var h = hD.substr(d & 15, 1);
        while(d > 15) {
            d >>= 4;
            h = hD.substr(d & 15, 1) + h;
        }
        return h;
    },

    random_color: function(red_min, red_max, green_min, green_max, blue_min, blue_max){
        var red = red_min + this.get_random(red_max - red_min);
        var green = green_min + this.get_random(green_max - green_min);
        var blue = blue_min + this.get_random(blue_max - blue_min);
        return "#" + this.dec2hex(red) + this.dec2hex(green) + this.dec2hex(blue);
    },

    play: function(){
        var t = document.getElementById("copyright_td" + (this.get_random(4) + 1));
        var r = this.random_color(180, 255, 150, 230, 150, 255);
        t.style.backgroundColor = r;
        var this_ = this;
        setTimeout(function(){this_.play()}, 500);
    },
}

copyright.play();