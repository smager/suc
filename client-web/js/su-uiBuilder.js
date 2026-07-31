(function (global) {    
    const exports =  function(sn) {
        // sn = "selectorName"
        let targetSelector = sn;
        let _self = this;
        let _pTemplates = localStorage.getItem("publicTemplates");
        let _$localTmpl = document.querySelectorAll("[id='localTemplates'],[name='localTemplates']");

        exports.templates ??= Object.create(null);
        const _templates = exports.templates;

        // Check if element is string selector, existing DOM element, or needs to be created
        if (typeof targetSelector === "string") {
            this.target = document.querySelector(targetSelector);
        } else if (targetSelector && targetSelector.nodeType) {
            this.target = targetSelector; 
        } else {
            this.target = document.createElement("div");
        }
        
        if (!this.target) {
            console.error("Selector name not found.");
            return;
        }
        
        this.lastObj = this.target;
        
        const _replaceTagElements = function(element) {
            let _r = element;
            let _cls = element.getAttribute("class");
            
            if (_cls && _cls.indexOf("tag-") > -1) {
                let match = _cls.match(/tag-(\w+)/);
                if (match) {
                    let _tag = match[1];
                    let newEl = document.createElement(_tag);
                    
                    // Copy classes but remove the "tag-*" prefix
                    let classList = Array.from(element.classList).filter(c => !c.startsWith("tag-"));
                    if (classList.length > 0) {
                        newEl.setAttribute("class", classList.join(" "));
                    }
                    
                    // Copy other attributes
                    Array.from(element.attributes).forEach(attr => {
                        if (attr.name !== "class" && attr.value !== '=""') {
                            newEl.setAttribute(attr.name, attr.value);
                        }
                    });
                    
                    // Transfer inner HTML
                    newEl.innerHTML = element.innerHTML;
                    
                    // Replace in DOM if it has a parent
                    if (element.parentNode) {
                        element.parentNode.replaceChild(newEl, element);
                    }
                    _r = newEl;
                }
            }
            return _r;
        };

        this.html = function(isNoEmpty) {
            let el = typeof sn === "string" ? document.querySelector(sn) : this.target;
            if (!el) return "";
            let _r = el.innerHTML;
            if (!isNoEmpty) el.innerHTML = ""; 
            return _r;
        };

        this.in = function() {
            _self.target = _self.lastObj;
            return this;
        };

        this.out = function() {
            if (_self.target.parentNode) {
                _self.target = _self.target.parentNode;
            }
            return this;
        };

        this.new = function() {
            let el = typeof sn === "string" ? document.querySelector(sn) : document.createElement('div');
            if (el) el.innerHTML = "";
            _self.target = el;
            _self.lastObj = null;
            return this;
        };

        this.custom = function(fn) {
            if (typeof fn === 'function') {
                fn.call(this); 
            }
            return this;
        };
        
       const _write = function (o) {

            o.html = o.html.replace(/srcx/g, "src");
            const temp = document.createElement("template");
            temp.innerHTML = o.html.trim();
            let node = temp.content.firstElementChild;
            if (!node)
                return;
            node = _replaceTagElements(node);
            let appendTarget = this.target;
            if (o.parent) {
                appendTarget =
                    typeof o.parent === "string"
                        ? document.querySelector(o.parent)
                        : o.parent;
            }
            if (appendTarget) {
                appendTarget.appendChild(node);
                this.lastObj = node;
            }
        };

        const _loadTemplates = function(source){
            let containers;
            if(typeof source === "string"){
                const div = document.createElement("div");
                div.innerHTML = source;
                containers = div.children;
            }else{
                containers = source;
            }

            [...containers].forEach(container=>{

                [...container.children].forEach(child=>{

                    const name = child.getAttribute("name");

                    if(!name)
                        return;

                    if(name in _templates)
                        return;

                    _templates[name] = child.innerHTML;

                });

            });

        };
        
        // Load Templates
        if (_pTemplates) _loadTemplates(_pTemplates);
        
        if (_$localTmpl.length > 0) {
            _loadTemplates(_$localTmpl);
            // Remove templates from DOM after loading
            _$localTmpl.forEach(el => el.remove());
        }

        // save methods
        Object.keys(_templates).forEach(name => {
            if (exports.prototype[name])
                return;

            exports.prototype[name] = function () {
                const a = arguments;
                let html = su.template.render(
                    _templates[name],
                    a[0] || {}
                );

                html = html
                    .replace(/(\w+\s?=\s?\"\")|(\w+\s?=\s?\'\')/g, "")
                    .replace(/\n/g, "");

                _write.call(this, {
                    html,
                    parent: a[1]
                });
                return this;

            };

        });
                
        return this;
    };

    if (!global) throw new Error("su-core.js is required.");
    global.uiBuilder = exports;     
})(su);