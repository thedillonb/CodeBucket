/*!
 * Copyright (c) 2011 Ivan Fomichev
 * http://github.com/codeholic/jscreole
 * Licensed under MIT license, see http://www.opensource.org/licenses/mit-license.php
 */

var creole = function(options) {
    var rx = {};
    rx._link = '[^\\]|~\\n]*(?:(?:\\](?!\\])|~.)[^\\]|~\\n]*)*';
    rx._linkText = '[^\\]~\\n]*(?:(?:\\](?!\\])|~.)[^\\]~\\n]*)*';
    rx._uriPrefix = '\\b(?:(?:https?|ftp)://|mailto:)';
    rx._uri = rx._uriPrefix + rx._link;
    rx._rawUri = rx._uriPrefix + '\\S*[^\\s!"\',.:;?]';
    rx._interwikiLink = '[\\w.]+:' + rx._link;
    rx._img = '\\{\\{((?!\\{)[^|}\\n]*(?:}(?!})[^|}\\n]*)*)' +
             (options && options.strict ? '' : '(?:') +
             '\\|([^}~\\n]*((}(?!})|~.)[^}~\\n]*)*)' +
             (options && options.strict ? '' : ')?') +
             '}}';

    var formatLink = function(link, format) {
        if (format instanceof Function) {
            return format(link);
        }

        format = format instanceof Array ? format : [ format ];
        if (typeof format[1] == 'undefined') { format[1] = ''; }
        return format[0] + link + format[1];
    };

    var g = {
        _hr: { _tag: 'hr', _regex: /(^|\n)\s*----\s*(\n|$)/ },

        _br: { _tag: 'br', _regex: /\\\\/ },

        _preBlock: { _tag: 'pre', _capture: 2,
            _regex: /(^|\n)\{\{\{\n((.*\n)*?)\}\}\}(\n|$)/,
            _replaceRegex: /^ ([ \t]*\}\}\})/gm,
            _replaceString: '$1' },
        _tt: { _tag: 'tt',
            _regex: /\{\{\{(.*?\}\}\}+)/, _capture: 1,
            _replaceRegex: /\}\}\}$/, _replaceString: '' },

        _ulist: { _tag: 'ul', _capture: 0,
            _regex: /(^|\n)([ \t]*\*[^*#].*(\n|$)([ \t]*[^\s*#].*(\n|$))*([ \t]*[*#]{2}.*(\n|$))*)+/ },
        _olist: { _tag: 'ol', _capture: 0,
            _regex: /(^|\n)([ \t]*#[^*#].*(\n|$)([ \t]*[^\s*#].*(\n|$))*([ \t]*[*#]{2}.*(\n|$))*)+/ },
        _li: { _tag: 'li', _capture: 0,
            _regex: /[ \t]*([*#]).+(\n[ \t]*[^*#\s].*)*(\n[ \t]*[*#]{2}.+)*/,
            _replaceRegex: /(^|\n)[ \t]*[*#]/g, _replaceString: '$1' },

        _table: { _tag: 'table', _capture: 0,
            _regex: /(^|\n)(\|.*?[ \t]*(\n|$))+/ },
        _tr: { _tag: 'tr', _capture: 2, _regex: /(^|\n)(\|.*?)\|?[ \t]*(\n|$)/ },
        _th: { _tag: 'th', _regex: /\|+=([^|]*)/, _capture: 1 },
        _td: { _tag: 'td', _capture: 1,
            _regex: '\\|+([^|~\\[{]*((~(.|(?=\\n)|$)|' +
                   '\\[\\[' + rx._link + '(\\|' + rx._linkText + ')?\\]\\]' +
                   (options && options.strict ? '' : '|' + rx._img) +
                   '|[\\[{])[^|~]*)*)' },

        _singleLine: { _regex: /.+/, _capture: 0 },
        _paragraph: { _tag: 'p', _capture: 0,
            _regex: /(^|\n)([ \t]*\S.*(\n|$))+/ },
        _text: { _capture: 0, _regex: /(^|\n)([ \t]*[^\s].*(\n|$))+/ },

        _strong: { _tag: 'strong', _capture: 1,
            _regex: /\*\*([^*~]*((\*(?!\*)|~(.|(?=\n)|$))[^*~]*)*)(\*\*|\n|$)/ },
        _em: { _tag: 'em', _capture: 1,
            _regex: '\\/\\/(((?!' + rx._uriPrefix + ')[^\\/~])*' +
                   '((' + rx._rawUri + '|\\/(?!\\/)|~(.|(?=\\n)|$))' +
                   '((?!' + rx._uriPrefix + ')[^\\/~])*)*)(\\/\\/|\\n|$)' },

        _img: { _regex: rx._img,
            _build: function(node, r, options) {
                var img = document.createElement('img');
                img.src = r[1];
                img.alt = r[2] === undefined
                    ? (options && options.defaultImageText ? options.defaultImageText : '')
                    : r[2].replace(/~(.)/g, '$1');
                node.appendChild(img);
            } },

        _namedUri: { _regex: '\\[\\[(' + rx._uri + ')\\|(' + rx._linkText + ')\\]\\]',
            _build: function(node, r, options) {
                var link = document.createElement('a');
                link.href = r[1];
                if (options && options.isPlainUri) {
                    link.appendChild(document.createTextNode(r[2]));
                }
                else {
                    this._apply(link, r[2], options);
                }
                node.appendChild(link);
            } },

        _namedLink: { _regex: '\\[\\[(' + rx._link + ')\\|(' + rx._linkText + ')\\]\\]',
            _build: function(node, r, options) {
                var link = document.createElement('a');

                link.href = options && options.linkFormat
                    ? formatLink(r[1].replace(/~(.)/g, '$1'), options.linkFormat)
                    : r[1].replace(/~(.)/g, '$1');
                this._apply(link, r[2], options);

                node.appendChild(link);
            } },

        _unnamedUri: { _regex: '\\[\\[(' + rx._uri + ')\\]\\]' },
        _unnamedLink: { _regex: '\\[\\[(' + rx._link + ')\\]\\]' },
        _unnamedInterwikiLink: { _regex: '\\[\\[(' + rx._interwikiLink + ')\\]\\]' },

        _rawUri: { _regex: '(' + rx._rawUri + ')' },

        _escapedSequence: { _regex: '~(' + rx._rawUri + '|.)', _capture: 1,
            _tag: 'span', _attrs: { 'class': 'escaped' } },
        _escapedSymbol: { _regex: /~(.)/, _capture: 1,
            _tag: 'span', _attrs: { 'class': 'escaped' } }
    };
    g._unnamedUri._build = g._rawUri._build = function(node, r, options) {
        if (!options) { options = {}; }
        options.isPlainUri = true;
        g._namedUri._build.call(this, node, Array(r[0], r[1], r[1]), options);
    };
    g._unnamedLink._build = function(node, r, options) {
        g._namedLink._build.call(this, node, Array(r[0], r[1], r[1]), options);
    };
    g._namedInterwikiLink = { _regex: '\\[\\[(' + rx._interwikiLink + ')\\|(' + rx._linkText + ')\\]\\]',
        _build: function(node, r, options) {
                var link = document.createElement('a');

                var m, f;
                if (options && options.interwiki) {
                m = r[1].match(/(.*?):(.*)/);
                f = options.interwiki[m[1]];
            }

            if (typeof f == 'undefined') {
                if (!g._namedLink._apply) {
                    g._namedLink = new this.constructor(g._namedLink);
                }
                return g._namedLink._build.call(g._namedLink, node, r, options);
            }

            link.href = formatLink(m[2].replace(/~(.)/g, '$1'), f);

            this._apply(link, r[2], options);

            node.appendChild(link);
        }
    };
    g._unnamedInterwikiLink._build = function(node, r, options) {
        g._namedInterwikiLink._build.call(this, node, Array(r[0], r[1], r[1]), options);
    };
    g._namedUri._children = g._unnamedUri._children = g._rawUri._children =
            g._namedLink._children = g._unnamedLink._children =
            g._namedInterwikiLink._children = g._unnamedInterwikiLink._children =
        [ g._escapedSymbol, g._img ];

    for (var i = 1; i <= 6; i++) {
        g['h' + i] = { _tag: 'h' + i, _capture: 2,
            _regex: '(^|\\n)[ \\t]*={' + i + '}[ \\t]*' +
                   '([^\\n=][^~]*?(~(.|(?=\\n)|$))*)[ \\t]*=*\\s*(\\n|$)'
        };
    }

    g._ulist._children = g._olist._children = [ g._li ];
    g._li._children = [ g._ulist, g._olist ];
    g._li._fallback = g._text;

    g._table._children = [ g._tr ];
    g._tr._children = [ g._th, g._td ];
    g._td._children = [ g._singleLine ];
    g._th._children = [ g._singleLine ];

    g.h1._children = g.h2._children = g.h3._children =
            g.h4._children = g.h5._children = g.h6._children =
            g._singleLine._children = g._paragraph._children =
            g._text._children = g._strong._children = g._em._children =
        [ g._escapedSequence, g._strong, g._em, g._br, g._rawUri,
            g._namedUri, g._namedInterwikiLink, g._namedLink,
            g._unnamedUri, g._unnamedInterwikiLink, g._unnamedLink,
            g._tt, g._img ];

    g._root = {
        _children: [ g.h1, g.h2, g.h3, g.h4, g.h5, g.h6,
            g._hr, g._ulist, g._olist, g._preBlock, g._table ],
        _fallback: { _children: [ g._paragraph ] }
    };

    creole._base.call(this, g, options);
};

creole._base = function(grammar, options) {
    if (!arguments.length) { return; }

    this._grammar = grammar;
    this._grammar._root = new this._ruleConstructor(this._grammar._root);
    this._options = options;
};

creole._rule = function(params) {
    if (!arguments.length) { return; }

    for (var p in params) { this[p] = params[p]; }
    if (!this._children) { this._children = []; }
};

creole._base.prototype = {
    _ruleConstructor: null,
    _grammar: null,
    _options: null,

    parse: function(node, data, options) {
        if (options) {
            for (i in this._options) {
                if (typeof options[i] == 'undefined') { options[i] = this._options[i]; }
            }
        }
        else {
            options = this._options;
        }
        data = data.replace(/\r\n?/g, '\n');
        this._grammar._root._apply(node, data, options);
        if (options && options.forIE) { node.innerHTML = node.innerHTML.replace(/\r?\n/g, '\r\n'); }
    }
};

creole._base.prototype.constructor = creole._base;

creole._base.prototype._ruleConstructor = creole._rule;

creole._rule.prototype = {
    _match: function(data, options) {
        return data.match(this._regex);
    },

    _build: function(node, r, options) {
        var data;
        if (this._capture !== null) {
            data = r[this._capture];
        }

        var target;
        if (this._tag) {
            target = document.createElement(this._tag);
            node.appendChild(target);
        }
        else { target = node; }

        if (data) {
            if (this._replaceRegex) {
                data = data.replace(this._replaceRegex, this._replaceString);
            }
            this._apply(target, data, options);
        }

        if (this._attrs) {
            for (var i in this._attrs) {
                target.setAttribute(i, this._attrs[i]);
                if (options && options.forIE && i == 'class') { target.className = this._attrs[i]; }
            }
        }
        return this;
    },

    _apply: function(node, data, options) {
        var tail = '' + data;
        var matches = [];

        if (!this._fallback._apply) {
            this._fallback = new this.constructor(this._fallback);
        }

        while (true) {
            var best = false;
            var rule  = false;
            for (var i = 0; i < this._children.length; i++) {
                if (typeof matches[i] == 'undefined') {
                    if (!this._children[i]._match) {
                        this._children[i] = new this.constructor(this._children[i]);
                    }
                    matches[i] = this._children[i]._match(tail, options);
                }
                if (matches[i] && (!best || best.index > matches[i].index)) {
                    best = matches[i];
                    rule = this._children[i];
                    if (best.index == 0) { break; }
                }
            }

            var pos = best ? best.index : tail.length;
            if (pos > 0) {
                this._fallback._apply(node, tail.substring(0, pos), options);
            }

            if (!best) { break; }

            if (!rule._build) { rule = new this.constructor(rule); }
            rule._build(node, best, options);

            var chopped = best.index + best[0].length;
            tail = tail.substring(chopped);
            for (var i = 0; i < this._children.length; i++) {
                if (matches[i]) {
                    if (matches[i].index >= chopped) {
                        matches[i].index -= chopped;
                    }
                    else {
                        matches[i] = void 0;
                    }
                }
            }
        }

        return this;
    },

    _fallback: {
        _apply: function(node, data, options) {
            if (options && options.forIE) {
                // workaround for bad IE
                data = data.replace(/\n/g, ' \r');
            }
            node.appendChild(document.createTextNode(data));
        }
    }
};

creole._rule.prototype.constructor = creole._rule;

creole.prototype = new creole._base();

creole.prototype.constructor = creole;

function convert(c) {

}
