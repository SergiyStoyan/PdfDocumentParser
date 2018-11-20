/************************************************************************
by Sergey Stoyan, 2018

This vanilla javascript generates a dynamic menu for content of a hosting html file when it is open in web browser.
Only this script with no dependency is required.
It can work either online or locally. 
Tested on Chrome and IE.


REQUIREMENTS:
Html body must have: 
- one <div class='header'>;
- one <div class='content'>;
- one <div class='footer'>;

Only <div class='content'> is parsed while building menu. Every H1, H2,... tag becomes an item.
 
 
USAGE:
Insert SCRIPT tag with menu_generator.js into the very end of HTML body.
Link menu_generator.css

 
AUXILIARY:
When scrolling the content, to find the currently visible item in the menu, click on its header in the content view.

To check a containing html file for broken internal links, open it in browser with anchor '#_checkInternalLinks'.

By default the header and the footer retain their initial positions but can be moved to the content view by setting attribute shiftHeaderAndFooterToContentView in the SCRIPT tag or property --shift-header-and-footer-to-content-view in key :root in menu_generator.css to a non-zero.
************************************************************************/
var convert = function(mode){
    var parseItemsFromContent = function(items, orderedItemIds){
        var ids = [0];
        var e = document.getElementsByClassName('content')[0].childNodes[0];
        while(e){//if(e.nodeType == Node.COMMENT_NODE)
            if(e.tagName){
                var m = e.tagName.match(/H(\d+)/i);
                if(m){
                    var level = parseInt(m[1]);
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
                    orderedItemIds.push(id);
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
        };
    };
    
    var menuContainer;
    
    var setModeSwithers = function(){
        var switchContainer = document.getElementsByClassName('switchContainer')[0];
        switchContainer.innerHTML = '<a class="switchLink" href="#_plainHtml" title="If the page is not displayed properly, switch to the plain html.">plain html</a>&nbsp;|&nbsp;' + (mode == '_collapsedContent' ? '<a class="switchLink" href="#_entireContent" title="Switch to the entire content mode.">entire content</a>' : '<a class="switchLink" href="#_collapsedContent" title="Switch to the collapsed content mode.">collapsed content</a>');
    };
         
    var addMenu2Page = function(){       
        var onClickMenuItem = function(){
            var id = this.getAttribute('_id');
            if(!id)
                return false;
            window.location.href = window.location.href.replace(/#.*/, '') + '#' + id;
            return false;
        };      
        
        var onClickItemHeader = function(){
            for(id in items)
                if(items[id]['header'] == this){
                    window.location.href = window.location.href.replace(/#.*/, '') + '#' + id;
                    return false;
                }
            return false;
        };
        
        var menu = document.createElement('div');
        menu.classList.add("menu");
        for(var i = 0; i < orderedItemIds.length; i++){
            var id = orderedItemIds[i];
            var e = document.createElement('span');
            if(/\S/.test(items[id]['content'].innerText)){
                e.classList.add('menuItem');
                e.addEventListener('click', onClickMenuItem);
            }
            else
                e.classList.add('menuNoclickItem');
            var level = (id.match(/_/ig) || []).length + 1;
            e.classList.add('h' + level);
            e.setAttribute('_id', id);
            e.innerHTML = items[id]['header'].innerText; 
            menu.appendChild(e);
            items[id]['menuItem'] = e;
            
            items[id]['header'].addEventListener('click', onClickItemHeader);
        }
        
        menuContainer = document.createElement('div');
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
        contentContainer.style.marginLeft = menuContainer.offsetWidth;
        content.parentNode.insertBefore(contentContainer, content);
        
        {//get headerAndFooterAreInContentContainer
            var headerAndFooterAreInContentContainer;
            var script;
            if(document.currentScript)
                script = document.currentScript;
            else{
                var ss = document.getElementsByTagName('script'); 
                script = ss[ss.length - 1];
            }
            headerAndFooterAreInContentContainer = parseInt(script.getAttribute('shiftHeaderAndFooterToContentView'))
                || parseInt(window.getComputedStyle(document.body).getPropertyValue('--shift-header-and-footer-to-content-view'));
        }
        
        var header = document.getElementsByClassName('header')[0];
        var footer = document.getElementsByClassName('footer')[0];
        
        if(headerAndFooterAreInContentContainer)
            contentContainer.appendChild(header);       
        
        contentContainer.appendChild(content);       
        
        if(headerAndFooterAreInContentContainer)
            contentContainer.appendChild(footer);
        
        {//manage content.style.minHeight
            if(headerAndFooterAreInContentContainer){//set the content minHeight to display the footer at the bottom
                content.style.minHeight = window.innerHeight + header.offsetTop - header.offsetHeight - footer.offsetHeight; 
            }
            else{   
                content.style.minHeight = window.innerHeight;   

                var isIE = /*@cc_on!@*/false || !!document.documentMode;
                if(!isIE)
                    window.onscroll = function(){//move menuContainer when scrolling at header or footer
                        var hr = header.getBoundingClientRect();
                        if(hr.bottom >= 0){
                            menuContainer.scrollTop -= menuContainer.getBoundingClientRect().top - hr.bottom;
                            menuContainer.style.top = hr.bottom;
                            menuContainer.style.height = window.innerHeight - hr.bottom;
                        }
                        else{                        
                            var fr = footer.getBoundingClientRect();
                            if(fr.top < window.innerHeight){                            
                                //menuContainer.style.top = fr.top - window.innerHeight;
                                menuContainer.scrollTop -= menuContainer.getBoundingClientRect().top;
                                menuContainer.style.top = 0;
                                menuContainer.style.height = fr.top;
                            }else{
                                menuContainer.scrollTop -= menuContainer.getBoundingClientRect().top;
                                menuContainer.style.top = 0;
                                menuContainer.style.height = window.innerHeight;
                            }
                        }
                    };
                else   //for IE, it scrolling is simplified to avoid jerking             
                    window.onscroll = function(){//move menuContainer when scrolling at header or footer
                        var hr = header.getBoundingClientRect();
                        if(hr.bottom >= 0){
                            //menuContainer.scrollTop -= menuContainer.getBoundingClientRect().top - hr.bottom;
                            menuContainer.style.top = hr.bottom;
                            menuContainer.style.height = window.innerHeight - hr.bottom;
                        }
                        else{                        
                            var fr = footer.getBoundingClientRect();
                            if(fr.top < window.innerHeight){                            
                                menuContainer.style.top = 0;
                                menuContainer.style.height = fr.top;
                            }else{
                                menuContainer.style.top = 0;
                                menuContainer.style.height = window.innerHeight;
                            }
                        }
                    };
            }
        }         
    }

    var navigate2currentAnchor = function(){
        var setItemVisibleInContent = function(item, visible){
            if(mode == '_collapsedContent')
                item['header'].classList.add('noTopMargin');
            else
                item['header'].classList.remove('noTopMargin');
            item['header'].style.display = visible ? 'block': 'none';
            item['content'].style.display = visible ? 'block': 'none';            
        };
        
        var openItem = function(item){
            menuContainer.style.top = 0;
            for(id in items)
                if(items[id] != item){
                    if(mode == '_collapsedContent')
                        setItemVisibleInContent(items[id], false);
                    else
                        setItemVisibleInContent(items[id], true);
                    items[id]['menuItem'].classList.remove('current');
                }
            if(item){
                setItemVisibleInContent(item, true);
                item['menuItem'].classList.add('current');               
                
                {//scroll the menu to get the current menu item visible
                    var menuContainerRect = menuContainer.getBoundingClientRect();   
                    var itemPosition = orderedItemIds.indexOf(item['id']);                    
                    var top;
                    if(itemPosition > 0)
                        top = items[orderedItemIds[itemPosition - 1]]['menuItem'].getBoundingClientRect().top;
                    else
                        top = - menuContainer.scrollTop;
                    if(top < 0)
                        menuContainer.scrollTop += top;
                    else{
                        var bottom;
                        if(itemPosition + 1 < orderedItemIds.length)
                            bottom = items[orderedItemIds[itemPosition + 1]]['menuItem'].getBoundingClientRect().bottom;
                        else
                            bottom = - menuContainer.scrollTop + menuContainer.scrollHeight + menuContainerRect.bottom;
                        if(bottom > menuContainerRect.bottom)
                            menuContainer.scrollTop += bottom - menuContainerRect.bottom;
                    }
                    
                    var itemRect = item['menuItem'].getBoundingClientRect();
                    if(itemRect.left < 0)
                        menuContainer.scrollLeft += itemRect.left;
                    else if(itemRect.right > menuContainerRect.right)
                        menuContainer.scrollLeft += itemRect.right - menuContainerRect.right;
                }                
                
                {//display also children until some one is not empty
                    //var level = (item['id'].match(/_/ig) || []).length + 1;
                    var i = orderedItemIds.indexOf(item['id']);
                    while(!/\S/.test(items[orderedItemIds[i]]['content'].innerText)){
                        i++;
                        if(i >= orderedItemIds.length)// || level >= (item['id'].match(/_/ig) || []).length + 1)
                            break;
                        setItemVisibleInContent(items[orderedItemIds[i]], true);
                    }
                }
                item['header'].scrollIntoView();
            }else{//no item to open
                document.getElementsByClassName('content')[0].scrollIntoView();//to trigger onScroll and set menu position
                window.scrollTo(0, 0);
                menuContainer.scrollTop = 0;
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
        openItem(false);
        return false;
    };

    var items = {};
    var orderedItemIds = [];
    parseItemsFromContent(items, orderedItemIds);
    addMenu2Page();

    var onHashchange = function(event){
        if(!navigate2currentAnchor())
            return true;
        event.preventDefault();
        return false;
    };
    window.addEventListener("hashchange", onHashchange, true);

    navigate2currentAnchor();

    {//it is only to prevent browser from unpleasant page jerking when navigating to an anchor which is hidden
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
        var internalAnchors = [];
        for(var i = 0; i < as.length; i++){
            if(as[i].href){
                var m = as[i].href.match(/.*#(.*)/);
                if(m)
                    internalLinks.push(m[1]);
            }
            if(as[i].name){
                internalAnchors.push(as[i].name);
            }            
        }
        internalLinks = internalLinks.filter(function(value, index, self){ return self.indexOf(value) === index; });
        var brokenLinks = [];
        for(var i = 0; i < internalLinks.length; i++){
            if(internalAnchors.indexOf(internalLinks[i]) < 0)
                brokenLinks.push(internalLinks[i]);
        }
        console.log('internalAnchors:', internalAnchors);
        console.log('internalLinks:', internalLinks);
        console.log('brokenLinks:', brokenLinks);
        if(brokenLinks.length){
            alert('There are broken internal links. They have been printed out on the console.');
            //console.log('Broken links:\r\n' + brokenLinks.join('\r\n'));
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