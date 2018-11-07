            
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
        
function onclickHeader(e){
    for(id in items)
        if(items[id]['menuItem'] == this){
            setVisible(items[id], true);
            items[id]['menuItem'].classList.add('current');
        }
        else{
            setVisible(items[id], false);//console.log(id);
            items[id]['menuItem'].classList.remove('current');
        }
    return false;
}
    
function createMenuElement(){
    var menu = document.createElement('div');
    menu.classList.add("menu");
    for(id in items){
        var level = id.match(/_/ig).length + 1;//alert(level);
        var e = document.createElement('span');
        e.classList.add('menuItem');
        e.classList.add('nobreak');
        e.classList.add('h' + level);
        e.setAttribute('_id', id);
        e.addEventListener('click', onclickHeader);
        e.innerHTML = items[id]['header'].innerHTML; 
        menu.appendChild(e);
        items[id]['menuItem'] = e;
        menu.appendChild(document.createElement('br'));
    }
    return menu;
}
var menu = createMenuElement();
var section = document.createElement('table');
section.innerHTML = '<tr><td class="menuTd"></td><td class="contentTd"></td></tr>';
//var section = document.createElement('section');
section.classList.add("section");
var content = document.getElementById('content');
content.parentNode.insertBefore(section, content);
//section.appendChild(menu);
//section.appendChild(content);
section.getElementsByTagName('td')[0].appendChild(menu);
section.getElementsByTagName('td')[1].appendChild(content);
for(i in items)
    setVisible(items[i], false);