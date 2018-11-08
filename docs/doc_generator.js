var getItems = function (e){
    var items = {};
    var ids = [0, 0];
    while(e){//alert(e);
        if(e.tagName)
        {
            var m = e.tagName.match(/H(\d+)/i);
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
                var content = document.createElement('div');
                e.parentNode.insertBefore(content, e.nextSibling);
                items[ids.join('_')] = {'header': e, 'content': content};
                e = content.nextSibling;
                continue;
            }
        }
        var item = items[ids.join('_')];
        if(item){
            item['content'].appendChild(e); 
            e = item['content'].nextSibling;
            continue;
        }
        e = e.nextSibling;
    }
    return items;
};
     
var addMenu2Page = function(){       
    var onclickMenuItem = function(){
        for(id in items)
            if(items[id]['menuItem'] == this){
                window.location.href = window.location.href.replace(/#.*/, '') + '#' + id;
                return false;
            }
        return false;
    };
    
    var menu = document.createElement('div');
    menu.classList.add("menu");
    for(id in items){
        var level = id.match(/_/ig).length + 1;//alert(level);
        var e = document.createElement('span');
        e.classList.add('menuItem');
        e.classList.add('nobreak');
        e.classList.add('h' + level);
        e.setAttribute('_id', id);
        e.addEventListener('click', onclickMenuItem);
        e.innerHTML = items[id]['header'].innerHTML; 
        menu.appendChild(e);
        items[id]['menuItem'] = e;
        //menu.appendChild(document.createElement('br'));
    }
    
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
}

var navigate2currentAnchor = function(){
    //alert(window.location.href);
    var setItemVisible = function(item, visible){
        item['header'].style.display = visible ? 'block': 'none';
        item['content'].style.display = visible ? 'block': 'none';
    };
    
    var openItem = function(item){
        for(id in items)
            if(items[id] != item){
                setItemVisible(items[id], false);//console.log(id);
                items[id]['menuItem'].classList.remove('current');
            }
        if(item){
            setItemVisible(item, true);
            item['menuItem'].classList.add('current');
        }
        window.scrollTo(0, 0);
    };
    
    var move2LocalAnchor = function(item, e, anchorName, isHeader){
        var as = e.getElementsByTagName('a');
        for(var i = 0; i < as.length; i++)
            if(as[i].name == anchorName){  //console.log(anchorName, as[i], item['menuItem']);
                openItem(item);
                if(!isHeader)
                    as[i].scrollIntoView();
                return true;
            }
        return false;
    }; 
    
    var anchorName = window.location.href.replace(/[^#]*#?(_localAnchor_)?/, '');//'_localAnchor_' was added to prevent browser from unpleasant page jerking when navigating to a hidden anchor
    //alert(anchorName);
    if(!anchorName){
        openItem(false);
        return;
    }
    if(items[anchorName]){
        openItem(items[anchorName]);
        return true;
    }
    for(var id in items){
        if(move2LocalAnchor(items[id], items[id]['header'], anchorName, true))
            return true;
        if(move2LocalAnchor(items[id], items[id]['content'], anchorName, false))
            return true;
    }
};


var items = getItems(document.getElementById('content').childNodes[0]);
addMenu2Page();

var onHashchange = function(event){
    if(!navigate2currentAnchor())
        return true;
    event.preventDefault();
    return false;
};
window.addEventListener("hashchange", onHashchange, true);

navigate2currentAnchor();

//it is only to prevent browser from unpleasant page jerking when navigating to an anchor which is hidden
var localPath = window.location.href.replace(/#.*/, '');
var as = content.getElementsByTagName('a');
for(var i = 0; i < as.length; i++){
    if(localPath != as[i].href.replace(/#.*/, ''))
        continue;
    var anchorName = as[i].href.replace(/^.*#/, '')
    if(!anchorName)
        continue;
    as[i].href = '#_localAnchor_' + anchorName;//this anchor does not really exists in the page
}       
//alert(1);