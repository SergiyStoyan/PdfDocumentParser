            
function getItems(e){
    var items = {};
    var ids = [0, 0];
    while(e){//alert(e);
        var m = false;
        if(e.tagName)
        {
            m = e.tagName.match(/H(\d+)/i);
            if(m)
            {
                var level = parseInt(m[1]);//alert(level);
                if(ids.length == level){
                }
                else if(ids.length < level){
                    while(ids.length < level)
                        ids.push(0);
                }
                else if(ids.length > level){
                    while(ids.length > level)
                        ids.pop();
                }
                ids[ids.length - 1] += 1;//alert(ids.join('_'));
                items[ids.join('_')] = {'header': e, 'content': []};
            }
        }
        if(!m){//console.log(e);//
            if(!e.style){//text node
                var s = document.createElement('span');//alert(e);
                e.parentNode.insertBefore(s, e.nextSibling);
                s.appendChild(e);            
                e = s;
            }
            if(items[ids.join('_')])
                items[ids.join('_')]['content'].push(e); 
        }
        e = e.nextSibling;
    }
    return items;
}
var items = getItems(document.getElementById('content').childNodes[0]);

function setVisible(item, visible){
    item['header'].style.display = visible ? 'block': 'none';
    for(ic in item['content']){
        item['content'][ic].style.display = visible ? 'block': 'none';
    }
}
        
function onclickHeader(id){
    for(i in items)
        setVisible(items[i], false);
    setVisible(items[id], true);
    var as = menu.getElementsByTagName('a');
    var r = new RegExp('#' + id + '$');
    for(i in as){
        if(!as[i].classList)
            continue;
        if(as[i].href.match(r)){//alert(1);
            as[i].classList.add("current");}
        else
            as[i].classList.remove("current");
    }
    return false;
}
    
function getMenuInnerHTML(items){
    var html = '';
    for(id in items){
        //var indent = '';
        var level = id.match(/_/ig).length + 1;//alert(level);
        //for(i = 1; i < level; i++)
        //    indent += '&nbsp;&nbsp;&nbsp;&nbsp;';
        html += '<br><span class="nobreak h' + level + '">' + '<a href="#' + id + '" onclick="return onclickHeader(\'' + id + '\');">' + items[id]['header'].innerHTML + '</a></span>';
    }
    return html;
}
var menu = document.createElement('div');
menu.classList.add("menu");
menu.innerHTML = getMenuInnerHTML(items, 2);
//var section = document.createElement('table');
//section.innerHTML = '<tr><td></td><td></td></tr>';
var section = document.createElement('section');
section.classList.add("section");
var content = document.getElementById('content');
content.parentNode.insertBefore(section, content);
section.appendChild(menu);
section.appendChild(content);
//section.getElementsByTagName('td')[0].appendChild(menu);
//section.getElementsByTagName('td')[1].appendChild(content);
for(i in items)
    setVisible(items[i], false);