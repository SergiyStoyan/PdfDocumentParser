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
    var header;
    var content;
    var footer;
    
    var setModeSwithers = function(){
        var switchContainer = document.getElementsByClassName('switchContainer')[0];
        switchContainer.innerHTML = '<a class="switchLink" href="#_plainHtml" title="If the page is not displayed properly, switch to the plain html.">plain html</a>&nbsp;|&nbsp;' + (mode == '_collapsedContent' ? '<a class="switchLink" href="#_entireContent" title="Switch to the entire content mode.">entire content</a>' : '<a class="switchLink" href="#_collapsedContent" title="Switch to the collapsed content mode.">collapsed content</a>');
    };
    
    var scrollMenuToCurrentItem = function(item)
    {//scroll the menu to get the current menu item visible
        if(!item){
            for(id in items)
                if(items[id]['menuItem'].classList.contains('current')){
                    item = items[id];
                    break;
                }                    
            if(!item)
                return;
        }
        
        var itemRect = item['menuItem'].getBoundingClientRect();
        
        var menuContainerRect = menuContainer.getBoundingClientRect();   
        var itemPosition = orderedItemIds.indexOf(item['id']);                    
        var top;
        var menuContainerRect = menuContainer.getBoundingClientRect();
        if(itemPosition > 0)
            top = items[orderedItemIds[itemPosition - 1]]['menuItem'].getBoundingClientRect().top;
        else
            //top = itemRect.top - itemRect.height;
            top = menuContainerRect.top - menuContainer.scrollTop;
        if(top <= menuContainerRect.top)
            menuContainer.scrollTop += top - menuContainerRect.top;
        else{
            var bottom;
            if(itemPosition + 1 < orderedItemIds.length)
                bottom = items[orderedItemIds[itemPosition + 1]]['menuItem'].getBoundingClientRect().bottom;
            else
                //bottom = itemRect.bottom + itemRect.height;
                bottom = menuContainerRect.bottom - menuContainer.scrollTop + menuContainer.scrollHeight;
            if(bottom >= menuContainerRect.bottom)
                menuContainer.scrollTop += bottom - menuContainerRect.bottom;
        }
        
        if(itemRect.left < 0)
            menuContainer.scrollLeft += itemRect.left;
        else if(itemRect.right > menuContainerRect.right)
            menuContainer.scrollLeft += itemRect.right - menuContainerRect.right;
    };                

    var setMenuContainerLocation; 
                  
    var addMenu2Page = function(){       
        var setWindowLocationHref = function(id){
            var href2 = window.location.href.replace(/#.*/, '') + '#' + id;
            if(window.location.href == href2)
                navigate2currentAnchor();//since it will not be triggered
            else
                window.location.href = href2;
        };
        
        var onClickMenuItem = function(){
            var id = this.getAttribute('_id');
            if(id)
                setWindowLocationHref(id);
            return false;
        };      
        
        var onClickItemHeader = function(){
            for(id in items)
                if(items[id]['header'] == this){
                    setWindowLocationHref(id);
                    return false;
                }
            return false;
        };
        
        var menu = document.createElement('div');
        menu.classList.add("menu");
        for(var i = 0; i < orderedItemIds.length; i++){
            var id = orderedItemIds[i];
            var e = document.createElement('span');
            e.classList.add('menuItem');
            if(/\S/.test(items[id]['content'].innerText)){
                e.addEventListener('click', onClickMenuItem);
            }
            else
                e.classList.add('empty');
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
        content = document.getElementsByClassName('content')[0];
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
        
        header = document.getElementsByClassName('header')[0];
        footer = document.getElementsByClassName('footer')[0];
        
        if(headerAndFooterAreInContentContainer)
            contentContainer.appendChild(header);       
        
        contentContainer.appendChild(content);       
        
        if(headerAndFooterAreInContentContainer)
            contentContainer.appendChild(footer);
        
        {//manage content.style.minHeight and set menu location while scrolling
            if(headerAndFooterAreInContentContainer){//set the content minHeight to display the footer at the bottom
                window.onresize = function(){
                    content.style.minHeight = window.innerHeight + header.offsetTop - header.offsetHeight - footer.offsetHeight; 
                }
            }
            else{     
                var isIE = /*@cc_on!@*/false || !!document.documentMode;
                var isEdge = !isIE && !!window.StyleMedia;
                if(!isIE && !isEdge)//works smoothly on Chrome
                    setMenuContainerLocation = function(){//move menuContainer when scrolling at header or footer
                        var hr = header.getBoundingClientRect();
                        if(hr.bottom >= 0){
                            menuContainer.scrollTop -= menuContainer.getBoundingClientRect().top - hr.bottom;
                            menuContainer.style.top = hr.bottom;
                            menuContainer.style.height = document.body.clientHeight/*window.innerHeight*/ - hr.bottom;//!!!window.innerHeight includes scrollbar if shown
                        }
                        else{                        
                            var fr = footer.getBoundingClientRect();
                            if(fr.top < document.body.clientHeight/*window.innerHeight*/){                           
                                menuContainer.scrollTop -= menuContainer.getBoundingClientRect().top;
                                menuContainer.style.top = 0;
                                menuContainer.style.height = fr.top;
                            }else{
                                menuContainer.scrollTop -= menuContainer.getBoundingClientRect().top;
                                menuContainer.style.top = 0;
                                menuContainer.style.height = document.body.clientHeight;//window.innerHeight;
                            }
                        }
                    };
                else   //for IE, scrolling is simplified to avoid jerking             
                    setMenuContainerLocation = function(){//move menuContainer when scrolling at header or footer
                        var hr = header.getBoundingClientRect();
                        if(hr.bottom >= 0){
                            menuContainer.style.top = hr.bottom;
                            menuContainer.style.height = document.body.clientHeight/*window.innerHeight*/ - hr.bottom;
                        }
                        else{                        
                            var fr = footer.getBoundingClientRect();
                            if(fr.top < document.body.clientHeight/*window.innerHeight*/){      
                                menuContainer.style.top = 0;
                                menuContainer.style.height = fr.top;
                            }else{
                                menuContainer.style.top = 0;
                                menuContainer.style.height = document.body.clientHeight;//window.innerHeight;
                            }
                        }
                    };            
                    // setMenuContainerLocation = function(){//move menuContainer when scrolling at header or footer
                        // var hr = header.getBoundingClientRect();
                        // if(hr.bottom >= 0){
                            // menuContainer.style.top = hr.bottom;
                            // menuContainer.style.height = document.body.clientHeight/*window.innerHeight*/ - hr.bottom;
                        // }
                        // else{                        
                            // var fr = footer.getBoundingClientRect();
                            // if(fr.top < document.body.clientHeight/*window.innerHeight*/){ 
                                // menuContainer.scrollTop -= fr.top - document.body.clientHeight;    
                                // menuContainer.style.top = 0;
                                // menuContainer.style.height = fr.top;
                            // }else{
                                // menuContainer.style.top = 0;
                                // menuContainer.style.height = document.body.clientHeight;//window.innerHeight;
                            // }
                        // }
                    // };
                window.onresize = function(){
                    content.style.minHeight = window.innerHeight; 
                    setMenuContainerLocation();
                    //scrollMenuToCurrentItem();//need to cure wrong shifting
                }
                window.onscroll = function(){
                    setMenuContainerLocation();
                    //scrollMenuToCurrentItem();//need to cure wrong shifting
                }
            }
            window.onresize();
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
        
        var openItem = function(item, anchor){
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
                
                { //scroll window only until displaying footer 
                    var left, top;
                    if(anchor){
                        var ar = anchor.getBoundingClientRect();   
                        left = ar.left;                     
                        top = ar.top;
                    }else{
                        left = 0;           
                        top = (mode == '_collapsedContent' ? content : item['header']).getBoundingClientRect().top;
                    }
                    top = Math.min(footer.getBoundingClientRect().top - window.innerHeight, top);
                    window.scrollTo(window.pageXOffset + left, window.pageYOffset + top);
                }
                
                scrollMenuToCurrentItem(item);
            }else{//no item to open
                window.scrollTo(0, 0);
                if(setMenuContainerLocation)
                    setMenuContainerLocation();
                menuContainer.scrollTop = 0;
            }
        };
        
        var findLocalAnchor = function(e, anchorName){
            var as = e.getElementsByTagName('a');
            for(var i = 0; i < as.length; i++)
                if(as[i].name == anchorName)  
                    return as[i];
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
                window.location.href = window.location.href.replace(/#.*/, '#');
            return true;
        }
        if(items[anchorName]){
            openItem(items[anchorName]);
            return true;
        }
        for(var id in items){//try to open as a real internal link
            var a = findLocalAnchor(items[id]['header'], anchorName);
            if(!a)
                a = findLocalAnchor(items[id]['content'], anchorName);
            if(a){
                openItem(items[id], a);
                return true;
            }
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
    //if(mode == '_collapsedContent'){ //show the header when opening first time       
        //window.scrollTo(0, 0);
        //scrollMenuToCurrentItem();
    //}

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
        body.insertBefore(anchorDiv, document.body.childNodes[0]);
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