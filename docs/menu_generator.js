/************************************************************************
by Sergey Stoyan, 2018

This vanilla javascript generates a dynamic menu from content of the hosting html file.
It was designed to work both online and locally. 
Tested on Chrome and IE.

REQUIREMENTS:
Html body must have: 
- one <div class='header'>;
- one <div class='content'>;
- one <div class='footer'>;
 Only <div class='content'> is parsed while building menu. 
 Every <h1>, <h2>... tag becomes an item.
 
USAGE:
Embed SCRIPT tag with menu_generator.js into the very end of HTML body.
Additionaly, link menu_generator.css
 
AUXILIARY:
Open a containing html file with anchor '#_checkInternalLinks' to check it for broken internal links.
************************************************************************/
var convert = function(mode){
    var getItemsFromContent = function(){
        var items = {};
        var ids = [0, 0];
        var e = document.getElementsByClassName('content')[0].childNodes[0];
        while(e){//if(e.nodeType == Node.COMMENT_NODE)
            if(e.tagName)
            {
                var m = e.tagName.match(/H(\d+)/i);
                if(m)
                {
                    var level = parseInt(m[1]) + 1;
                    if(ids.length < level){
                        while(ids.length < level)
                            ids.push(0);
                    }
                    else if(ids.length > level){
                        while(ids.length > level)
                            ids.pop();
                    }
                    ids[ids.length - 1] += 1;
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
    
    var setModeSwithers = function(){
        var switchContainer = document.getElementsByClassName('switchContainer')[0];
        switchContainer.innerHTML = '<a class="switchLink" href="#_plainHtml" title="If the page is not displayed properly, switch to the plain html.">plain html</a>&nbsp;|&nbsp;' + (mode == '_collapsedContent' ? '<a class="switchLink" href="#_entireContent" title="Switch to the entire content mode.">entire content</a>' : '<a class="switchLink" href="#_collapsedContent" title="Switch to the collapsed content mode.">collapsed content</a>');
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
            var level = id.match(/_/ig).length + 1;
            var e = document.createElement('span');
            e.classList.add('menuItem');
            e.classList.add('h' + level);
            e.setAttribute('_id', id);
            e.addEventListener('click', onclickMenuItem);
            e.innerHTML = items[id]['header'].innerText; 
            menu.appendChild(e);
            items[id]['menuItem'] = e;
        }
        
        var menuContainer = document.createElement('div');
        menuContainer.classList.add('menuContainer');
        var switchContainer = document.createElement('div');
        switchContainer.classList.add('switchContainer');
        menuContainer.appendChild(switchContainer);
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
        var setItemVisible = function(item, visible){
            item['header'].style.display = visible ? 'block': 'none';
            item['content'].style.display = visible ? 'block': 'none';
        };
        
        var openItem = function(item){
            for(id in items)
                if(items[id] != item){
                    if(mode == '_collapsedContent')
                        setItemVisible(items[id], false);
                    else
                        setItemVisible(items[id], true);
                    items[id]['menuItem'].classList.remove('current');
                }
            if(item){
                setItemVisible(item, true);
                item['menuItem'].classList.add('current');               
                
                {//scroll the menu to get the current menu item visible
                    var r = item['menuItem'].getBoundingClientRect();
                    var menuContainer = document.getElementsByClassName('menuContainer')[0];
                    var pr = menuContainer.getBoundingClientRect();                
                    //if(r.top < 0 || r.bottom > pr.bottom)
                    //    item['menuItem'].scrollIntoView();
                    if(r.top < 0)
                        menuContainer.scrollTop += r.top;
                    else if(r.bottom > pr.bottom)
                        menuContainer.scrollTop += r.bottom - pr.bottom;
                    if(r.left < 0)
                        menuContainer.scrollLeft += r.left;
                    else if(r.right > pr.right)
                        menuContainer.scrollLeft += r.right - pr.right;
                }                
                
                {//display also children until some one is not empty
                    var id = item['id'];
                    var childIds = [id];
                    var childId = id;
                    while(!/\S/.test(items[childId]['content'].innerText)){
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
        };
        
        var openLocalAnchor = function(item, e, anchorName, isHeader){
            var as = e.getElementsByTagName('a');
            for(var i = 0; i < as.length; i++)
                if(as[i].name == anchorName){  
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
            case '_collapsedContent':
                mode = anchorName;
                setModeSwithers();
                for(id in items)
                    if(items[id]['menuItem'].classList.contains('current')){
                        window.location.href = window.location.href.replace(/#.*/, '#' + id);
                        return true;
                    }
                    alert(window.location.href);
                window.location.href = window.location.href.replace(/#.*/, '#');
            return true;
        }
        if(items[anchorName]){
            openItem(items[anchorName]);
            return true;
        }
        for(var id in items){
            if(openLocalAnchor(items[id], items[id]['header'], anchorName, true))
                return true;
            if(openLocalAnchor(items[id], items[id]['content'], anchorName, false))
                return true;
        }
        return false;
    };

    var items = getItemsFromContent();
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

var anchorName = window.location.href.replace(/[^#]*#?(_localAnchor_)?/, '');
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
    case '_checkInternalLinks':
        var as = document.getElementsByTagName('a');
        var internalLinks = [];
        var internalAnchorNames = [];
        for(var i = 0; i < as.length; i++){
            if(as[i].href){
                var m = as[i].href.match(/^\s*#(.*)/);
                if(m)
                    internalLinks.push(m[1]);
            }
            if(as[i].name){
                internalAnchorNames.push(as[i].name);
            }            
        }
        var brokenLinks = [];
        for(var i = 0; i < internalLinks.length; i++){
            if(internalAnchorNames.indexOf(internalLinks[i]) < 0)
                brokenLinks.push(internalLinks[i]);
        }
        if(brokenLinks.length){
            alert('There are broken internal links. They have been printed out on the console.');
            console.log('Broken links:\r\n' + brokenLinks.join('\r\n'));
        }else
            alert('No broken internal link was found.');
    break;
    case '_entireContent':
        convert('_entireContent');
    break;
    default:
        convert('_collapsedContent');
    break;
}
//alert(1);