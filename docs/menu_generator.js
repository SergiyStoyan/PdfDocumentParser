/*by Sergey Stoyan, 2018

This vanilla javascipt generates a dynamic menu from a html. 
It was designed to work both online and locally. 
Tested on Chrome and IE.

REQUIREMENTS:
Html body must have: 
- 1 <div class='header'>;
- 1 <div class='content'>;
- 1 <div class='footer'>;
 Only <div class='content'> is parsed while building menu. 
 Every <h1>, <h2>... tag becomes an item.
 
 USAGE:
 This script must be embedded in the end of html.
 Also menu_generator.css must be linked.
*/
var convert = function(mode){
    var getItems = function(){
        var items = {};
        var ids = [0, 0];
        var e = document.getElementsByClassName('content')[0].childNodes[0];
        while(e){//if(e.nodeType == Node.COMMENT_NODE)console.log(e.data);
            if(e.tagName)
            {
                var m = e.tagName.match(/H(\d+)/i);
                if(m)
                {
                    var level = parseInt(m[1]) + 1;//alert(level);
                    if(ids.length < level){
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
                    var id = ids.join('_');
                    items[id] = {'header': e, 'content': content, 'id': id};
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
    
    var setModeSwithers = function(){//alert(mode+(mode == '_collapsedContent'));
        var switcherContainer = document.getElementsByClassName('switcherContainer')[0];
        switcherContainer.innerHTML = '<a class="switchLink" href="#_plainHtml" title="If the page is not displayed properly, switch to the plain html.">plain html</a>&nbsp;|&nbsp;' + (mode == '_collapsedContent' ? '<a class="switchLink" href="#_entireContent" title="Switch to the entire content mode.">entire content</a>' : '<a class="switchLink" href="#_collapsedContent" title="Switch to the collapsed content mode.">collapsed content</a>');
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
            //e.classList.add('nobreak');
            e.classList.add('h' + level);
            e.setAttribute('_id', id);
            e.addEventListener('click', onclickMenuItem);
            e.innerHTML = items[id]['header'].innerText; 
            menu.appendChild(e);
            items[id]['menuItem'] = e;
            //menu.appendChild(document.createElement('br'));
        }
        
        var menuContainer = document.createElement('div');
        menuContainer.classList.add("menuContainer");
        var switcherContainer = document.createElement('div');
        switcherContainer.classList.add('switcherContainer');
        menuContainer.appendChild(switcherContainer);
        menuContainer.appendChild(menu);
        var content = document.getElementsByClassName('content')[0];
        content.parentNode.insertBefore(menuContainer, content);
        setModeSwithers();
        
        var contentContainer = document.createElement('div');
        contentContainer.classList.add("contentContainer");
        content.parentNode.insertBefore(contentContainer, content);
        //contentContainer.appendChild(document.getElementsByClassName('header')[0]);       
        contentContainer.appendChild(content);       
        //contentContainer.appendChild(document.getElementsByClassName('footer')[0]);       
        contentContainer.style.marginLeft = menuContainer.offsetWidth;
        
        var getOuterHeight = function(e){
            var ss = window.getComputedStyle(e);
            return e.offsetHeight + parseInt(ss['marginTop']) + parseInt(ss['marginBottom']);
        };        
        contentContainer.style.minHeight = window.innerHeight + getOuterHeight(document.getElementsByClassName('header')[0]) - getOuterHeight(document.getElementsByClassName('footer')[0]);
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
                    if(mode == '_collapsedContent')
                        setItemVisible(items[id], false);//console.log(id);
                    items[id]['menuItem'].classList.remove('current');
                }
            if(item){
                setItemVisible(item, true);
                item['menuItem'].classList.add('current');
                {//display also children until something is not empty
                    var id = item['id'];
                    var childIds = [id];
                    var childId = id;
                    while(!/\S/.test(items[childId]['content'].innerHTML)){
                        childIds.push(1);
                        childId = childIds.join('_');
                        while(!items[childId]){
                            childIds.pop();
                            if(childIds.length == 1)
                                break;
                            childIds[childIds.length - 1] += 1;
                            childId = childIds.join('_');
                        }
                        if(!items[childId])
                            break;
                        setItemVisible(items[childId], true);
                    }
                }
                item['header'].scrollIntoView();
            }
            //window.scrollTo(0, 0);
            //document.getElementsByClassName('content')[0].scrollIntoView();
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
        if(!anchorName){
            openItem(false);
            return true;
        }
        switch(anchorName){
            case '_plainHtml':
                location.reload();
            return true;
            case '_entireContent':
                mode = '_entireContent';
                setModeSwithers();
                var currentItem;
                for(id in items){
                    if(items[id]['menuItem'].classList.contains('current'))
                        currentItem = items[id];
                    setItemVisible(items[id], true);
                }
                if(currentItem)
                    currentItem['header'].scrollIntoView();
            return true;
            case '_collapsedContent':
                mode = '_collapsedContent';
                setModeSwithers();
                for(id in items){
                    if(items[id]['menuItem'].classList.contains('current')){
                        setItemVisible(items[id], true);
                        currentItem = items[id];
                    }else
                        setItemVisible(items[id], false);
                }
                if(currentItem)
                    currentItem['header'].scrollIntoView();
            return true;
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
        return false;
    };

    var items = getItems();
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
    var as = document.getElementsByTagName('a');
    for(var i = 0; i < as.length; i++){
        if(localPath != as[i].href.replace(/#.*/, ''))
            continue;
        var anchorName = as[i].href.replace(/^.*#/, '')
        if(!anchorName)
            continue;
        as[i].href = '#_localAnchor_' + anchorName;//this anchor does not really exists in the page and so browser will not try to go there
    }  
};

var anchorName = window.location.href.replace(/[^#]*#?(_localAnchor_)?/, '');//alert(anchorName);
switch(anchorName){
    case '_plainHtml':
        var anchorDiv = document.createElement('div');
        var loadInMenuMode = function(){
            var localPath = window.location.href.replace(/#.*/, '');
            window.location.href = localPath;
            location.reload();
            return false;
        };
        anchorDiv.innerHTML = '<a class="switchLink" href="#" onclick="loadInMenuMode();" title="Switch to javascript generated document.">menu mode</a>';
        var body = document.getElementsByTagName('body')[0];
        body.insertBefore(anchorDiv, body.childNodes[0]);
    break;
    case '_entireContent':
        convert('_entireContent');
    break;
    default:
        convert('_collapsedContent');
    break;
}
//alert(1);