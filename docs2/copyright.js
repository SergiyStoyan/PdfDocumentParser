{
    var css = '\
div.copyright { border1: 1px solid grey; margin:0; align-self:center; }\
table.copyright { margin:0; align-self:center; }\
table.copyright, table.copyright td {border:none; }\
table.copyright td {margin:0; padding:0; width:6px; height:6px;}',
        head = document.head || document.getElementsByTagName('head')[0],
        style = document.createElement('style');
    style.type = 'text/css';
    if (style.styleSheet)// This is required for IE8 and below.
      style.styleSheet.cssText = css;
    else 
      style.appendChild(document.createTextNode(css));
    head.appendChild(style);
}

document.write('\
<div class="copyright">\
    <a href="http://cliversoft.com" title="cliversoft.com">\
        <TABLE class="copyright">\
            <TR>\
                <TD id="copyright_td1"></TD>\
                <TD id="copyright_td2"></TD>\
            </TR>\
            <TR>\
                <TD id="copyright_td3"></TD>\
                <TD id="copyright_td4"></TD>\
            </TR>\
        </TABLE>\
     </a>\
 </div>\
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
        var r = this.random_color(120, 230, 120, 230, 120, 230);
        t.style.backgroundColor = r;
        var this_ = this;
        setTimeout(function(){this_.play()}, 500);
    },
}

copyright.play();