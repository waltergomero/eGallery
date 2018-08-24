!function(t,e){var o=function(t){var e={};function o(i){if(e[i])return e[i].exports;var n=e[i]={i:i,l:!1,exports:{}};return t[i].call(n.exports,n,n.exports,o),n.l=!0,n.exports}return o.m=t,o.c=e,o.d=function(t,e,i){o.o(t,e)||Object.defineProperty(t,e,{configurable:!1,enumerable:!0,get:i})},o.r=function(t){Object.defineProperty(t,"__esModule",{value:!0})},o.n=function(t){var e=t&&t.__esModule?function(){return t.default}:function(){return t};return o.d(e,"a",e),e},o.o=function(t,e){return Object.prototype.hasOwnProperty.call(t,e)},o.p="",o(o.s=256)}({1:function(t,e){t.exports=window.jQuery},159:function(t,e,o){
/*!
 *
 * Jquery Mapael - Dynamic maps jQuery plugin (based on raphael.js)
 * Requires jQuery, raphael.js and jquery.mousewheel
 *
 * Version: 2.2.0
 *
 * Copyright (c) 2017 Vincent Brouté (https://www.vincentbroute.fr/mapael)
 * Licensed under the MIT license (http://www.opensource.org/licenses/mit-license.php).
 *
 * Thanks to Indigo744
 *
 */
t.exports=function(t,e,o,i){"use strict";var n,a,s=function(e,o){this.container=e,this.$container=t(e),this.options=this.extendDefaultOptions(o),this.zoomTO=0,this.zoomCenterX=0,this.zoomCenterY=0,this.previousPinchDist=0,this.zoomData={zoomLevel:0,zoomX:0,zoomY:0,panX:0,panY:0},this.currentViewBox={x:0,y:0,w:0,h:0},this.panning=!1,this.zoomAnimID=null,this.zoomAnimStartTime=null,this.zoomAnimCVBTarget=null,this.$map=t("."+this.options.map.cssClass,this.container),this.initialMapHTMLContent=this.$map.html(),this.$tooltip={},this.paper={},this.areas={},this.plots={},this.links={},this.legends={},this.mapConf={},this.customEventHandlers={},this.init()};return s.prototype={MouseOverFilteringTO:120,panningFilteringTO:150,panningEndFilteringTO:50,zoomFilteringTO:150,resizeFilteringTO:150,init:function(){var o=this;if(""===o.options.map.cssClass||0===t("."+o.options.map.cssClass,o.container).length)throw new Error("The map class `"+o.options.map.cssClass+"` doesn't exists");if(o.$tooltip=t("<div>").addClass(o.options.map.tooltip.cssClass).css("display","none"),o.$map.empty().append(o.$tooltip),t.mapael&&t.mapael.maps&&t.mapael.maps[o.options.map.name])o.mapConf=t.mapael.maps[o.options.map.name];else{if(!(t.fn.mapael&&t.fn.mapael.maps&&t.fn.mapael.maps[o.options.map.name]))throw new Error("Unknown map '"+o.options.map.name+"'");o.mapConf=t.fn.mapael.maps[o.options.map.name],window.console&&window.console.warn&&window.console.warn("Extending $.fn.mapael is deprecated (map '"+o.options.map.name+"')")}if(o.paper=new e(o.$map[0],o.mapConf.width,o.mapConf.height),!0===o.isRaphaelBBoxBugPresent())throw o.destroy(),new Error("Can't get boundary box for text (is your container hidden? See #135)");o.$container.addClass("mapael"),o.options.map.tooltip.css&&o.$tooltip.css(o.options.map.tooltip.css),o.setViewBox(0,0,o.mapConf.width,o.mapConf.height),o.options.map.width?o.paper.setSize(o.options.map.width,o.mapConf.height*(o.options.map.width/o.mapConf.width)):o.initResponsiveSize(),t.each(o.mapConf.elems,function(t){o.areas[t]={},o.areas[t].options=o.getElemOptions(o.options.map.defaultArea,o.options.areas[t]?o.options.areas[t]:{},o.options.legend.area),o.areas[t].mapElem=o.paper.path(o.mapConf.elems[t])}),o.options.map.beforeInit&&o.options.map.beforeInit(o.$container,o.paper,o.options),t.each(o.mapConf.elems,function(t){o.initElem(t,"area",o.areas[t])}),o.links=o.drawLinksCollection(o.options.links),t.each(o.options.plots,function(t){o.plots[t]=o.drawPlot(t)}),o.$container.on("zoom.mapael",function(t,e){o.onZoomEvent(t,e)}),o.options.map.zoom.enabled&&o.initZoom(o.mapConf.width,o.mapConf.height,o.options.map.zoom),o.options.map.zoom.init!==i&&(o.options.map.zoom.init.animDuration===i&&(o.options.map.zoom.init.animDuration=0),o.$container.trigger("zoom",o.options.map.zoom.init)),o.createLegends("area",o.areas,1),o.createLegends("plot",o.plots,o.paper.width/o.mapConf.width),o.$container.on("update.mapael",function(t,e){o.onUpdateEvent(t,e)}),o.$container.on("showElementsInRange.mapael",function(t,e){o.onShowElementsInRange(t,e)}),o.initDelegatedMapEvents(),o.initDelegatedCustomEvents(),o.options.map.afterInit&&o.options.map.afterInit(o.$container,o.paper,o.areas,o.plots,o.options),t(o.paper.desc).append(" and Mapael "+o.version+" (https://www.vincentbroute.fr/mapael/)")},destroy:function(){var e=this;e.$container.off(".mapael"),e.$map.off(".mapael"),e.onResizeEvent&&t(window).off("resize.mapael",e.onResizeEvent),e.$map.empty(),e.$map.html(e.initialMapHTMLContent),t.each(e.legends,function(o){t.each(e.legends[o],function(t){var i=e.legends[o][t];i.container.empty(),i.container.html(i.initialHTMLContent)})}),e.$container.removeClass("mapael"),e.$container.removeData("mapael"),e.container=i,e.$container=i,e.options=i,e.paper=i,e.$map=i,e.$tooltip=i,e.mapConf=i,e.areas=i,e.plots=i,e.links=i,e.customEventHandlers=i},initResponsiveSize:function(){var e=this,o=null,i=function(t){var o=e.$map.width();if(e.paper.width!==o){var i=o/e.mapConf.width;e.paper.setSize(o,e.mapConf.height*i),!0!==t&&e.options.legend.redrawOnResize&&e.createLegends("plot",e.plots,i)}};e.onResizeEvent=function(){clearTimeout(o),o=setTimeout(function(){i()},e.resizeFilteringTO)},t(window).on("resize.mapael",e.onResizeEvent),i(!0)},extendDefaultOptions:function(e){return e=t.extend(!0,{},s.prototype.defaultOptions,e),t.each(["area","plot"],function(o,i){if(t.isArray(e.legend[i]))for(var n=0;n<e.legend[i].length;++n)e.legend[i][n]=t.extend(!0,{},s.prototype.legendDefaultOptions[i],e.legend[i][n]);else e.legend[i]=t.extend(!0,{},s.prototype.legendDefaultOptions[i],e.legend[i])}),e},initDelegatedMapEvents:function(){var e,o,n=this,a={area:n.areas,"area-text":n.areas,plot:n.plots,"plot-text":n.plots,link:n.links,"link-text":n.links};n.$container.on("mouseover.mapael","[data-id]",function(){var o=this;clearTimeout(e),e=setTimeout(function(){var e=t(o),s=e.attr("data-id"),r=e.attr("data-type");if(a[r]!==i)n.elemEnter(a[r][s]);else if("legend-elem"===r||"legend-label"===r){var l=e.attr("data-legend-id"),p=e.attr("data-legend-type");n.elemEnter(n.legends[p][l].elems[s])}},n.MouseOverFilteringTO)}),n.$container.on("mousemove.mapael","[data-id]",function(e){var s=this;clearTimeout(o),o=setTimeout(function(){var o=t(s),r=o.attr("data-id"),l=o.attr("data-type");a[l]!==i&&n.elemHover(a[l][r],e)},0)}),n.$container.on("mouseout.mapael","[data-id]",function(){clearTimeout(e),clearTimeout(o);var s=t(this),r=s.attr("data-id"),l=s.attr("data-type");if(a[l]!==i)n.elemOut(a[l][r]);else if("legend-elem"===l||"legend-label"===l){var p=s.attr("data-legend-id"),m=s.attr("data-legend-type");n.elemOut(n.legends[m][p].elems[r])}}),n.$container.on("click.mapael","[data-id]",function(e,o){var s=t(this),r=s.attr("data-id"),l=s.attr("data-type");if(a[l]!==i)n.elemClick(a[l][r]);else if("legend-elem"===l||"legend-label"===l){var p=s.attr("data-legend-id"),m=s.attr("data-legend-type");n.handleClickOnLegendElem(n.legends[m][p].elems[r],r,p,m,o)}})},initDelegatedCustomEvents:function(){var e=this;t.each(e.customEventHandlers,function(o){var n=o+".mapael.custom";e.$container.off(n).on(n,"[data-id]",function(n){var a=t(this),s=a.attr("data-id"),r=a.attr("data-type").replace("-text","");if(!e.panning&&e.customEventHandlers[o][r]!==i&&e.customEventHandlers[o][r][s]!==i){var l=e.customEventHandlers[o][r][s];l.options.eventHandlers[o](n,s,l.mapElem,l.textElem,l.options)}})})},initElem:function(e,o,n){var a=t(n.mapElem.node);if(n.options.href&&(n.options.attrs.cursor="pointer",n.options.text&&(n.options.text.attrs.cursor="pointer")),n.mapElem.attr(n.options.attrs),a.attr({"data-id":e,"data-type":o}),n.options.cssClass!==i&&a.addClass(n.options.cssClass),n.options.text&&n.options.text.content!==i){var s=this.getTextPosition(n.mapElem.getBBox(),n.options.text.position,n.options.text.margin);n.options.text.attrs.text=n.options.text.content,n.options.text.attrs.x=s.x,n.options.text.attrs.y=s.y,n.options.text.attrs["text-anchor"]=s.textAnchor,n.textElem=this.paper.text(s.x,s.y,n.options.text.content),n.textElem.attr(n.options.text.attrs),t(n.textElem.node).attr({"data-id":e,"data-type":o+"-text"})}n.options.eventHandlers&&this.setEventHandlers(e,o,n),this.setHoverOptions(n.mapElem,n.options.attrs,n.options.attrsHover),n.textElem&&this.setHoverOptions(n.textElem,n.options.text.attrs,n.options.text.attrsHover)},initZoom:function(e,o,n){var a=this,s=!1,r=0,l=0,p={reset:function(){a.$container.trigger("zoom",{level:0})},in:function(){a.$container.trigger("zoom",{level:"+1"})},out:function(){a.$container.trigger("zoom",{level:-1})}};t.extend(a.zoomData,{zoomLevel:0,panX:0,panY:0}),t.each(n.buttons,function(e,o){if(p[e]===i)throw new Error("Unknown zoom button '"+e+"'");var n=t("<div>").addClass(o.cssClass).html(o.content).attr("title",o.title);n.on("click.mapael",p[e]),a.$map.append(n)}),a.options.map.zoom.mousewheel&&a.$map.on("mousewheel.mapael",function(t){var e=t.deltaY>0?1:-1,o=a.mapPagePositionToXY(t.pageX,t.pageY);a.$container.trigger("zoom",{fixedCenter:!0,level:a.zoomData.zoomLevel+e,x:o.x,y:o.y}),t.preventDefault()}),a.options.map.zoom.touch&&(a.$map.on("touchstart.mapael",function(t){2===t.originalEvent.touches.length&&(a.zoomCenterX=(t.originalEvent.touches[0].pageX+t.originalEvent.touches[1].pageX)/2,a.zoomCenterY=(t.originalEvent.touches[0].pageY+t.originalEvent.touches[1].pageY)/2,a.previousPinchDist=Math.sqrt(Math.pow(t.originalEvent.touches[1].pageX-t.originalEvent.touches[0].pageX,2)+Math.pow(t.originalEvent.touches[1].pageY-t.originalEvent.touches[0].pageY,2)))}),a.$map.on("touchmove.mapael",function(t){var e=0,o=0;if(2===t.originalEvent.touches.length){if(e=Math.sqrt(Math.pow(t.originalEvent.touches[1].pageX-t.originalEvent.touches[0].pageX,2)+Math.pow(t.originalEvent.touches[1].pageY-t.originalEvent.touches[0].pageY,2)),Math.abs(e-a.previousPinchDist)>15){var i=a.mapPagePositionToXY(a.zoomCenterX,a.zoomCenterY);o=(e-a.previousPinchDist)/Math.abs(e-a.previousPinchDist),a.$container.trigger("zoom",{fixedCenter:!0,level:a.zoomData.zoomLevel+o,x:i.x,y:i.y}),a.previousPinchDist=e}return!1}})),a.$map.on("dragstart",function(){return!1});var m=null,h=null;t("body").on("mouseup.mapael"+(n.touch?" touchend.mapael":""),function(){s=!1,clearTimeout(m),clearTimeout(h),m=setTimeout(function(){a.panning=!1},a.panningEndFilteringTO)}),a.$map.on("mousedown.mapael"+(n.touch?" touchstart.mapael":""),function(t){clearTimeout(m),clearTimeout(h),t.pageX!==i?(s=!0,r=t.pageX,l=t.pageY):1===t.originalEvent.touches.length&&(s=!0,r=t.originalEvent.touches[0].pageX,l=t.originalEvent.touches[0].pageY)}).on("mousemove.mapael"+(n.touch?" touchmove.mapael":""),function(p){var c=a.zoomData.zoomLevel,d=0,u=0;if(clearTimeout(m),clearTimeout(h),p.pageX!==i?(d=p.pageX,u=p.pageY):1===p.originalEvent.touches.length?(d=p.originalEvent.touches[0].pageX,u=p.originalEvent.touches[0].pageY):s=!1,s&&0!==c){var f=(r-d)/(1+c*n.step)*(e/a.paper.width),g=(l-u)/(1+c*n.step)*(o/a.paper.height),w=Math.min(Math.max(0,a.currentViewBox.x+f),e-a.currentViewBox.w),x=Math.min(Math.max(0,a.currentViewBox.y+g),o-a.currentViewBox.h);return(Math.abs(f)>5||Math.abs(g)>5)&&(t.extend(a.zoomData,{panX:w,panY:x,zoomX:w+a.currentViewBox.w/2,zoomY:x+a.currentViewBox.h/2}),a.setViewBox(w,x,a.currentViewBox.w,a.currentViewBox.h),h=setTimeout(function(){a.$map.trigger("afterPanning",{x1:w,y1:x,x2:w+a.currentViewBox.w,y2:x+a.currentViewBox.h})},a.panningFilteringTO),r=d,l=u,a.panning=!0),!1}})},mapPagePositionToXY:function(t,e){var o=this.$map.offset(),i=this.options.map.width?this.mapConf.width/this.options.map.width:this.mapConf.width/this.$map.width(),n=1/(1+this.zoomData.zoomLevel*this.options.map.zoom.step);return{x:n*i*(t-o.left)+this.zoomData.panX,y:n*i*(e-o.top)+this.zoomData.panY}},onZoomEvent:function(e,o){var n,a,s,r,l,p=this,m=p.zoomData.zoomLevel,h=1+p.zoomData.zoomLevel*p.options.map.zoom.step,c=o.animDuration!==i?o.animDuration:p.options.map.zoom.animDuration;if(o.area!==i){if(p.areas[o.area]===i)throw new Error("Unknown area '"+o.area+"'");var d=o.areaMargin!==i?o.areaMargin:10,u=p.areas[o.area].mapElem.getBBox(),f=u.width+2*d,g=u.height+2*d;o.x=u.cx,o.y=u.cy,m=Math.min(Math.floor((p.mapConf.width/f-1)/p.options.map.zoom.step),Math.floor((p.mapConf.height/g-1)/p.options.map.zoom.step))}else if(o.level!==i&&(m="string"==typeof o.level?"+"===o.level.slice(0,1)||"-"===o.level.slice(0,1)?p.zoomData.zoomLevel+parseInt(o.level,10):parseInt(o.level,10):o.level<0?p.zoomData.zoomLevel+o.level:o.level),o.plot!==i){if(p.plots[o.plot]===i)throw new Error("Unknown plot '"+o.plot+"'");o.x=p.plots[o.plot].coords.x,o.y=p.plots[o.plot].coords.y}else{if(o.latitude!==i&&o.longitude!==i){var w=p.mapConf.getCoords(o.latitude,o.longitude);o.x=w.x,o.y=w.y}o.x===i&&(o.x=p.currentViewBox.x+p.currentViewBox.w/2),o.y===i&&(o.y=p.currentViewBox.y+p.currentViewBox.h/2)}l=1+(m=Math.min(Math.max(m,p.options.map.zoom.minLevel),p.options.map.zoom.maxLevel))*p.options.map.zoom.step,s=p.mapConf.width/l,r=p.mapConf.height/l,0===m?(n=0,a=0):(o.fixedCenter!==i&&!0===o.fixedCenter?(n=p.zoomData.panX+(o.x-p.zoomData.panX)*(l-h)/l,a=p.zoomData.panY+(o.y-p.zoomData.panY)*(l-h)/l):(n=o.x-s/2,a=o.y-r/2),n=Math.min(Math.max(0,n),p.mapConf.width-s),a=Math.min(Math.max(0,a),p.mapConf.height-r)),l===h&&n===p.zoomData.panX&&a===p.zoomData.panY||(c>0?p.animateViewBox(n,a,s,r,c,p.options.map.zoom.animEasing):(p.setViewBox(n,a,s,r),clearTimeout(p.zoomTO),p.zoomTO=setTimeout(function(){p.$map.trigger("afterZoom",{x1:n,y1:a,x2:n+s,y2:a+r})},p.zoomFilteringTO)),t.extend(p.zoomData,{zoomLevel:m,panX:n,panY:a,zoomX:n+s/2,zoomY:a+r/2}))},onShowElementsInRange:function(t,e){e.animDuration===i&&(e.animDuration=0),e.hiddenOpacity===i&&(e.hiddenOpacity=.3),e.ranges&&e.ranges.area&&this.showElemByRange(e.ranges.area,this.areas,e.hiddenOpacity,e.animDuration),e.ranges&&e.ranges.plot&&this.showElemByRange(e.ranges.plot,this.plots,e.hiddenOpacity,e.animDuration),e.ranges&&e.ranges.link&&this.showElemByRange(e.ranges.link,this.links,e.hiddenOpacity,e.animDuration),e.afterShowRange&&e.afterShowRange()},showElemByRange:function(e,o,n,a){var s=this,r={};e.min===i&&e.max===i||(e={0:e}),t.each(e,function(a){var s=e[a];if(s.min===i&&s.max===i)return!0;t.each(o,function(t){var e=o[t].options.value;if("object"!=typeof e&&(e=[e]),e[a]===i)return!0;s.min!==i&&e[a]<s.min||s.max!==i&&e[a]>s.max?r[t]=n:r[t]=1})}),t.each(r,function(t){s.setElementOpacity(o[t],r[t],a)})},setElementOpacity:function(t,e,o){e>0&&(t.mapElem.show(),t.textElem&&t.textElem.show()),this.animate(t.mapElem,{opacity:e},o,function(){0===e&&t.mapElem.hide()}),this.animate(t.textElem,{opacity:e},o,function(){0===e&&t.textElem.hide()})},onUpdateEvent:function(e,o){var n=this;if("object"==typeof o){var a=0,s=o.animDuration?o.animDuration:0,r=function(t){n.animate(t.mapElem,{opacity:0},s,function(){t.mapElem.remove()}),n.animate(t.textElem,{opacity:0},s,function(){t.textElem.remove()})},l=function(t){t.mapElem.attr({opacity:0}),t.textElem&&t.textElem.attr({opacity:0}),n.setElementOpacity(t,t.mapElem.originalAttrs.opacity!==i?t.mapElem.originalAttrs.opacity:1,s)};if("object"==typeof o.mapOptions&&(!0===o.replaceOptions?n.options=n.extendDefaultOptions(o.mapOptions):t.extend(!0,n.options,o.mapOptions),o.mapOptions.areas===i&&o.mapOptions.plots===i&&o.mapOptions.legend===i||t("[data-type='legend-elem']",n.$container).each(function(e,o){"1"===t(o).attr("data-hidden")&&t(o).trigger("click",{hideOtherElems:!1,animDuration:s})})),"object"==typeof o.deletePlotKeys)for(;a<o.deletePlotKeys.length;a++)n.plots[o.deletePlotKeys[a]]!==i&&(r(n.plots[o.deletePlotKeys[a]]),delete n.plots[o.deletePlotKeys[a]]);else"all"===o.deletePlotKeys&&(t.each(n.plots,function(t,e){r(e)}),n.plots={});if("object"==typeof o.deleteLinkKeys)for(a=0;a<o.deleteLinkKeys.length;a++)n.links[o.deleteLinkKeys[a]]!==i&&(r(n.links[o.deleteLinkKeys[a]]),delete n.links[o.deleteLinkKeys[a]]);else"all"===o.deleteLinkKeys&&(t.each(n.links,function(t,e){r(e)}),n.links={});if("object"==typeof o.newPlots&&t.each(o.newPlots,function(t){n.plots[t]===i&&(n.options.plots[t]=o.newPlots[t],n.plots[t]=n.drawPlot(t),s>0&&l(n.plots[t]))}),"object"==typeof o.newLinks){var p=n.drawLinksCollection(o.newLinks);t.extend(n.links,p),t.extend(n.options.links,o.newLinks),s>0&&t.each(p,function(t){l(p[t])})}if(t.each(n.areas,function(t){("object"==typeof o.mapOptions&&("object"==typeof o.mapOptions.map&&"object"==typeof o.mapOptions.map.defaultArea||"object"==typeof o.mapOptions.areas&&"object"==typeof o.mapOptions.areas[t]||"object"==typeof o.mapOptions.legend&&"object"==typeof o.mapOptions.legend.area)||!0===o.replaceOptions)&&(n.areas[t].options=n.getElemOptions(n.options.map.defaultArea,n.options.areas[t]?n.options.areas[t]:{},n.options.legend.area),n.updateElem(n.areas[t],s))}),t.each(n.plots,function(t){("object"==typeof o.mapOptions&&("object"==typeof o.mapOptions.map&&"object"==typeof o.mapOptions.map.defaultPlot||"object"==typeof o.mapOptions.plots&&"object"==typeof o.mapOptions.plots[t]||"object"==typeof o.mapOptions.legend&&"object"==typeof o.mapOptions.legend.plot)||!0===o.replaceOptions)&&(n.plots[t].options=n.getElemOptions(n.options.map.defaultPlot,n.options.plots[t]?n.options.plots[t]:{},n.options.legend.plot),n.setPlotCoords(n.plots[t]),n.setPlotAttributes(n.plots[t]),n.updateElem(n.plots[t],s))}),t.each(n.links,function(t){("object"==typeof o.mapOptions&&("object"==typeof o.mapOptions.map&&"object"==typeof o.mapOptions.map.defaultLink||"object"==typeof o.mapOptions.links&&"object"==typeof o.mapOptions.links[t])||!0===o.replaceOptions)&&(n.links[t].options=n.getElemOptions(n.options.map.defaultLink,n.options.links[t]?n.options.links[t]:{},{}),n.updateElem(n.links[t],s))}),o.mapOptions&&("object"==typeof o.mapOptions.legend||"object"==typeof o.mapOptions.map&&"object"==typeof o.mapOptions.map.defaultArea||"object"==typeof o.mapOptions.map&&"object"==typeof o.mapOptions.map.defaultPlot)&&(t("[data-type='legend-elem']",n.$container).each(function(e,o){"1"===t(o).attr("data-hidden")&&t(o).trigger("click",{hideOtherElems:!1,animDuration:s})}),n.createLegends("area",n.areas,1),n.options.map.width?n.createLegends("plot",n.plots,n.options.map.width/n.mapConf.width):n.createLegends("plot",n.plots,n.$map.width()/n.mapConf.width)),"object"==typeof o.setLegendElemsState)t.each(o.setLegendElemsState,function(e,o){var a=n.$container.find("."+e)[0];a!==i&&t("[data-type='legend-elem']",a).each(function(e,i){("0"===t(i).attr("data-hidden")&&"hide"===o||"1"===t(i).attr("data-hidden")&&"show"===o)&&t(i).trigger("click",{hideOtherElems:!1,animDuration:s})})});else{var m="hide"===o.setLegendElemsState?"hide":"show";t("[data-type='legend-elem']",n.$container).each(function(e,o){("0"===t(o).attr("data-hidden")&&"hide"===m||"1"===t(o).attr("data-hidden")&&"show"===m)&&t(o).trigger("click",{hideOtherElems:!1,animDuration:s})})}n.initDelegatedCustomEvents(),o.afterUpdate&&o.afterUpdate(n.$container,n.paper,n.areas,n.plots,n.options,n.links)}},setPlotCoords:function(t){if(t.options.x!==i&&t.options.y!==i)t.coords={x:t.options.x,y:t.options.y};else if(t.options.plotsOn!==i&&this.areas[t.options.plotsOn]!==i){var e=this.areas[t.options.plotsOn].mapElem.getBBox();t.coords={x:e.cx,y:e.cy}}else t.coords=this.mapConf.getCoords(t.options.latitude,t.options.longitude)},setPlotAttributes:function(t){"square"===t.options.type?(t.options.attrs.width=t.options.size,t.options.attrs.height=t.options.size,t.options.attrs.x=t.coords.x-t.options.size/2,t.options.attrs.y=t.coords.y-t.options.size/2):"image"===t.options.type?(t.options.attrs.src=t.options.url,t.options.attrs.width=t.options.width,t.options.attrs.height=t.options.height,t.options.attrs.x=t.coords.x-t.options.width/2,t.options.attrs.y=t.coords.y-t.options.height/2):"svg"===t.options.type?(t.options.attrs.path=t.options.path,t.options.attrs.transform===i&&(t.options.attrs.transform=""),t.mapElem.originalBBox===i&&(t.mapElem.originalBBox=t.mapElem.getBBox()),t.mapElem.baseTransform="m"+t.options.width/t.mapElem.originalBBox.width+",0,0,"+t.options.height/t.mapElem.originalBBox.height+","+(t.coords.x-t.options.width/2)+","+(t.coords.y-t.options.height/2),t.options.attrs.transform=t.mapElem.baseTransform+t.options.attrs.transform):(t.options.attrs.x=t.coords.x,t.options.attrs.y=t.coords.y,t.options.attrs.r=t.options.size/2)},drawLinksCollection:function(e){var o=this,n={},a={},s={},r={},l={};return t.each(e,function(t){var p=o.getElemOptions(o.options.map.defaultLink,e[t],{});if(n="string"==typeof e[t].between[0]?o.options.plots[e[t].between[0]]:e[t].between[0],a="string"==typeof e[t].between[1]?o.options.plots[e[t].between[1]]:e[t].between[1],n.plotsOn!==i&&o.areas[n.plotsOn]!==i){var m=o.areas[n.plotsOn].mapElem.getBBox();s={x:m.cx,y:m.cy}}else n.latitude!==i&&n.longitude!==i?s=o.mapConf.getCoords(n.latitude,n.longitude):(s.x=n.x,s.y=n.y);if(a.plotsOn!==i&&o.areas[a.plotsOn]!==i){var h=o.areas[a.plotsOn].mapElem.getBBox();r={x:h.cx,y:h.cy}}else a.latitude!==i&&a.longitude!==i?r=o.mapConf.getCoords(a.latitude,a.longitude):(r.x=a.x,r.y=a.y);l[t]=o.drawLink(t,s.x,s.y,r.x,r.y,p)}),l},drawLink:function(t,e,o,i,n,a){var s={options:a},r=(e+i)/2,l=(o+n)/2,p=-1/((n-o)/(i-e)),m=l-p*r,h=Math.sqrt((i-e)*(i-e)+(n-o)*(n-o)),c=1+p*p,d=-2*r+2*p*m-2*p*l,u=d*d-4*c*(r*r+m*m-m*l-l*m+l*l-a.factor*h*(a.factor*h)),f=0,g=0;return g=a.factor>0?p*(f=(-d+Math.sqrt(u))/(2*c))+m:p*(f=(-d-Math.sqrt(u))/(2*c))+m,s.mapElem=this.paper.path("m "+e+","+o+" C "+f+","+g+" "+i+","+n+" "+i+","+n),this.initElem(t,"link",s),s},isAttrsChanged:function(t,e){for(var o in e)if(e.hasOwnProperty(o)&&void 0===t[o]||e[o]!==t[o])return!0;return!1},updateElem:function(e,o){var n,a,s;if(!0===e.options.toFront&&e.mapElem.toFront(),e.options.href!==i?(e.options.attrs.cursor="pointer",e.options.text&&(e.options.text.attrs.cursor="pointer")):"pointer"===e.mapElem.attrs.cursor&&(e.options.attrs.cursor="auto",e.options.text&&(e.options.text.attrs.cursor="auto")),e.textElem){e.options.text.attrs.text=e.options.text.content,n=e.mapElem.getBBox(),(e.options.size||e.options.width&&e.options.height)&&("image"===e.options.type||"svg"===e.options.type?(a=(e.options.width-n.width)/2,s=(e.options.height-n.height)/2):(a=(e.options.size-n.width)/2,s=(e.options.size-n.height)/2),n.x-=a,n.x2+=a,n.y-=s,n.y2+=s);var r=this.getTextPosition(n,e.options.text.position,e.options.text.margin);e.options.text.attrs.x=r.x,e.options.text.attrs.y=r.y,e.options.text.attrs["text-anchor"]=r.textAnchor,this.setHoverOptions(e.textElem,e.options.text.attrs,e.options.text.attrsHover),this.isAttrsChanged(e.textElem.attrs,e.options.text.attrs)&&this.animate(e.textElem,e.options.text.attrs,o)}this.setHoverOptions(e.mapElem,e.options.attrs,e.options.attrsHover),this.isAttrsChanged(e.mapElem.attrs,e.options.attrs)&&this.animate(e.mapElem,e.options.attrs,o),e.options.cssClass!==i&&t(e.mapElem.node).removeClass().addClass(e.options.cssClass)},drawPlot:function(t){var e={};return e.options=this.getElemOptions(this.options.map.defaultPlot,this.options.plots[t]?this.options.plots[t]:{},this.options.legend.plot),this.setPlotCoords(e),"svg"===e.options.type&&(e.mapElem=this.paper.path(e.options.path)),this.setPlotAttributes(e),"square"===e.options.type?e.mapElem=this.paper.rect(e.options.attrs.x,e.options.attrs.y,e.options.attrs.width,e.options.attrs.height):"image"===e.options.type?e.mapElem=this.paper.image(e.options.attrs.src,e.options.attrs.x,e.options.attrs.y,e.options.attrs.width,e.options.attrs.height):"svg"===e.options.type||(e.mapElem=this.paper.circle(e.options.attrs.x,e.options.attrs.y,e.options.attrs.r)),this.initElem(t,"plot",e),e},setEventHandlers:function(e,o,n){var a=this;t.each(n.options.eventHandlers,function(t){a.customEventHandlers[t]===i&&(a.customEventHandlers[t]={}),a.customEventHandlers[t][o]===i&&(a.customEventHandlers[t][o]={}),a.customEventHandlers[t][o][e]=n})},drawLegend:function(o,n,a,s,r){var l={},p={},m=0,h=0,c=null,d=null,u={},f=0,g=0,w=0,x=0,v=[],y=(l=t("."+o.cssClass,this.$container)).html();for(l.empty(),p=new e(l.get(0)),t(p.canvas).attr({"data-legend-type":n,"data-legend-id":r}),h=m=0,o.title&&""!==o.title&&(d=(c=p.text(o.marginLeftTitle,0,o.title).attr(o.titleAttrs)).getBBox(),c.attr({y:.5*d.height}),m=o.marginLeftTitle+d.width,h+=o.marginBottomTitle+d.height),f=0;f<o.slices.length;++f){var E=0;v[f]=t.extend(!0,{},"plot"===n?this.options.map.defaultPlot:this.options.map.defaultArea,o.slices[f]),o.slices[f].legendSpecificAttrs===i&&(o.slices[f].legendSpecificAttrs={}),t.extend(!0,v[f].attrs,o.slices[f].legendSpecificAttrs),"area"===n?(v[f].attrs.width===i&&(v[f].attrs.width=30),v[f].attrs.height===i&&(v[f].attrs.height=20)):"square"===v[f].type?(v[f].attrs.width===i&&(v[f].attrs.width=v[f].size),v[f].attrs.height===i&&(v[f].attrs.height=v[f].size)):"image"===v[f].type||"svg"===v[f].type?(v[f].attrs.width===i&&(v[f].attrs.width=v[f].width),v[f].attrs.height===i&&(v[f].attrs.height=v[f].height)):v[f].attrs.r===i&&(v[f].attrs.r=v[f].size/2),E=o.marginBottomTitle,c&&(E+=d.height),"plot"!==n||v[f].type!==i&&"circle"!==v[f].type?E+=s*v[f].attrs.height/2:E+=s*v[f].attrs.r,x=Math.max(x,E)}for("horizontal"===o.mode&&(m=o.marginLeft),f=0;f<v.length;++f){var z={},C={},O={};if(v[f].display===i||!0===v[f].display){if("area"===n?("horizontal"===o.mode?(g=m+o.marginLeft,w=x-.5*s*v[f].attrs.height):(g=o.marginLeft,w=h),z=p.rect(g,w,s*v[f].attrs.width,s*v[f].attrs.height)):"square"===v[f].type?("horizontal"===o.mode?(g=m+o.marginLeft,w=x-.5*s*v[f].attrs.height):(g=o.marginLeft,w=h),z=p.rect(g,w,s*v[f].attrs.width,s*v[f].attrs.height)):"image"===v[f].type||"svg"===v[f].type?("horizontal"===o.mode?(g=m+o.marginLeft,w=x-.5*s*v[f].attrs.height):(g=o.marginLeft,w=h),"image"===v[f].type?z=p.image(v[f].url,g,w,s*v[f].attrs.width,s*v[f].attrs.height):(z=p.path(v[f].path),v[f].attrs.transform===i&&(v[f].attrs.transform=""),C=z.getBBox(),v[f].attrs.transform="m"+s*v[f].width/C.width+",0,0,"+s*v[f].height/C.height+","+g+","+w+v[f].attrs.transform)):("horizontal"===o.mode?(g=m+o.marginLeft+s*v[f].attrs.r,w=x):(g=o.marginLeft+s*v[f].attrs.r,w=h+s*v[f].attrs.r),z=p.circle(g,w,s*v[f].attrs.r)),delete v[f].attrs.width,delete v[f].attrs.height,delete v[f].attrs.r,z.attr(v[f].attrs),C=z.getBBox(),"horizontal"===o.mode?(g=m+o.marginLeft+C.width+o.marginLeftLabel,w=x):(g=o.marginLeft+C.width+o.marginLeftLabel,w=h+C.height/2),O=p.text(g,w,v[f].label).attr(o.labelAttrs),"horizontal"===o.mode){var b=o.marginBottom+C.height;m+=o.marginLeft+C.width+o.marginLeftLabel+O.getBBox().width,"image"!==v[f].type&&"area"!==n&&(b+=o.marginBottomTitle),c&&(b+=d.height),h=Math.max(h,b)}else m=Math.max(m,o.marginLeft+C.width+o.marginLeftLabel+O.getBBox().width),h+=o.marginBottom+C.height;t(z.node).attr({"data-legend-id":r,"data-legend-type":n,"data-type":"legend-elem","data-id":f,"data-hidden":0}),t(O.node).attr({"data-legend-id":r,"data-legend-type":n,"data-type":"legend-label","data-id":f,"data-hidden":0}),u[f]={mapElem:z,textElem:O},o.hideElemsOnClick.enabled&&(O.attr({cursor:"pointer"}),z.attr({cursor:"pointer"}),this.setHoverOptions(z,v[f].attrs,v[f].attrs),this.setHoverOptions(O,o.labelAttrs,o.labelAttrsHover),v[f].clicked!==i&&!0===v[f].clicked&&this.handleClickOnLegendElem(u[f],f,r,n,{hideOtherElems:!1}))}}return"SVG"!==e.type&&o.VMLWidth&&(m=o.VMLWidth),p.setSize(m,h),{container:l,initialHTMLContent:y,elems:u}},handleClickOnLegendElem:function(e,o,n,a,s){var r,l=this;s=s||{},r=t.isArray(l.options.legend[a])?l.options.legend[a][n]:l.options.legend[a];var p=e.mapElem,m=e.textElem,h=t(p.node),c=t(m.node),d=r.slices[o],u="area"===a?l.areas:l.plots,f=s.animDuration!==i?s.animDuration:r.hideElemsOnClick.animDuration,g=h.attr("data-hidden"),w="0"===g?{"data-hidden":"1"}:{"data-hidden":"0"};"0"===g?l.animate(m,{opacity:.5},f):l.animate(m,{opacity:1},f),t.each(u,function(e){var o,a=u[e].mapElem.data("hidden-by");a===i&&(a={}),o=t.isArray(u[e].options.value)?u[e].options.value[n]:u[e].options.value,l.getLegendSlice(o,r)===d&&("0"===g?(a[n]=!0,l.setElementOpacity(u[e],r.hideElemsOnClick.opacity,f)):(delete a[n],t.isEmptyObject(a)&&l.setElementOpacity(u[e],u[e].mapElem.originalAttrs.opacity!==i?u[e].mapElem.originalAttrs.opacity:1,f)),u[e].mapElem.data("hidden-by",a))}),h.attr(w),c.attr(w),s.hideOtherElems!==i&&!0!==s.hideOtherElems||!0!==r.exclusive||t("[data-type='legend-elem'][data-hidden=0]",l.$container).each(function(){var e=t(this);e.attr("data-id")!==o&&e.trigger("click",{hideOtherElems:!1})})},createLegends:function(e,o,i){var n=this.options.legend[e];t.isArray(this.options.legend[e])||(n=[this.options.legend[e]]),this.legends[e]={};for(var a=0;a<n.length;++a)!0===n[a].display&&t.isArray(n[a].slices)&&n[a].slices.length>0&&""!==n[a].cssClass&&0!==t("."+n[a].cssClass,this.$container).length&&(this.legends[e][a]=this.drawLegend(n[a],e,o,i,a))},setHoverOptions:function(o,i,n){"SVG"!==e.type&&delete n.transform,o.attrsHover=n,o.attrsHover.transform?o.originalAttrs=t.extend({transform:"s1"},i):o.originalAttrs=i},elemEnter:function(t){if(t!==i){if(t.mapElem!==i&&this.animate(t.mapElem,t.mapElem.attrsHover,t.mapElem.attrsHover.animDuration),t.textElem!==i&&this.animate(t.textElem,t.textElem.attrsHover,t.textElem.attrsHover.animDuration),t.options&&t.options.tooltip!==i){var e="";this.$tooltip.removeClass().addClass(this.options.map.tooltip.cssClass),t.options.tooltip.content!==i&&(e="function"==typeof t.options.tooltip.content?t.options.tooltip.content(t.mapElem):t.options.tooltip.content),t.options.tooltip.cssClass!==i&&this.$tooltip.addClass(t.options.tooltip.cssClass),this.$tooltip.html(e).css("display","block")}t.mapElem===i&&t.textElem===i||this.paper.safari&&this.paper.safari()}},elemHover:function(t,e){if(t!==i&&t.options.tooltip!==i){var o=e.pageX,n=e.pageY,a=10,s=20;"object"==typeof t.options.tooltip.offset&&(void 0!==t.options.tooltip.offset.left&&(a=t.options.tooltip.offset.left),void 0!==t.options.tooltip.offset.top&&(s=t.options.tooltip.offset.top));var r={left:Math.min(this.$map.width()-this.$tooltip.outerWidth()-5,o-this.$map.offset().left+a),top:Math.min(this.$map.height()-this.$tooltip.outerHeight()-5,n-this.$map.offset().top+s)};"object"==typeof t.options.tooltip.overflow&&(!0===t.options.tooltip.overflow.right&&(r.left=o-this.$map.offset().left+10),!0===t.options.tooltip.overflow.bottom&&(r.top=n-this.$map.offset().top+20)),this.$tooltip.css(r)}},elemOut:function(t){t!==i&&(t.mapElem!==i&&this.animate(t.mapElem,t.mapElem.originalAttrs,t.mapElem.attrsHover.animDuration),t.textElem!==i&&this.animate(t.textElem,t.textElem.originalAttrs,t.textElem.attrsHover.animDuration),t.options&&t.options.tooltip!==i&&this.$tooltip.css({display:"none",top:-1e3,left:-1e3}),t.mapElem===i&&t.textElem===i||this.paper.safari&&this.paper.safari())},elemClick:function(t){t!==i&&(this.panning||t.options.href===i||window.open(t.options.href,t.options.target))},getElemOptions:function(e,o,n){var a=t.extend(!0,{},e,o);if(a.value!==i)if(t.isArray(n))for(var s=0;s<n.length;++s)a=t.extend(!0,{},a,this.getLegendSlice(a.value[s],n[s]));else a=t.extend(!0,{},a,this.getLegendSlice(a.value,n));return a},getTextPosition:function(t,e,o){var i=0,n=0,a="";switch("number"==typeof o&&(o="bottom"===e||"top"===e?{x:0,y:o}:"right"===e||"left"===e?{x:o,y:0}:{x:0,y:0}),e){case"bottom":i=(t.x+t.x2)/2+o.x,n=t.y2+o.y,a="middle";break;case"top":i=(t.x+t.x2)/2+o.x,n=t.y-o.y,a="middle";break;case"left":i=t.x-o.x,n=(t.y+t.y2)/2+o.y,a="end";break;case"right":i=t.x2+o.x,n=(t.y+t.y2)/2+o.y,a="start";break;default:i=(t.x+t.x2)/2+o.x,n=(t.y+t.y2)/2+o.y,a="middle"}return{x:i,y:n,textAnchor:a}},getLegendSlice:function(t,e){for(var o=0;o<e.slices.length;++o)if(e.slices[o].sliceValue!==i&&t===e.slices[o].sliceValue||e.slices[o].sliceValue===i&&(e.slices[o].min===i||t>=e.slices[o].min)&&(e.slices[o].max===i||t<=e.slices[o].max))return e.slices[o];return{}},animateViewBox:function(t,o,i,n,a,s){var r=this,l=r.currentViewBox.x,p=t-l,m=r.currentViewBox.y,h=o-m,c=r.currentViewBox.w,d=i-c,u=r.currentViewBox.h,f=n-u;r.zoomAnimCVBTarget||(r.zoomAnimCVBTarget={x:t,y:o,w:i,h:n});var g=c>i?"in":"out",w=e.easing_formulas[s||"linear"],x=a-2*a/100,v=r.zoomAnimStartTime;r.zoomAnimStartTime=(new Date).getTime();var y=function(){r.cancelAnimationFrame(r.zoomAnimID);var e=(new Date).getTime()-r.zoomAnimStartTime;if(e<x){var s,E,z,C;if(v&&r.zoomAnimCVBTarget&&r.zoomAnimCVBTarget.w!==i){var O=(new Date).getTime()-v,b=w(O/a);s=l+(r.zoomAnimCVBTarget.x-l)*b,E=m+(r.zoomAnimCVBTarget.y-m)*b,z=c+(r.zoomAnimCVBTarget.w-c)*b,C=u+(r.zoomAnimCVBTarget.h-u)*b,p=t-(l=s),h=o-(m=E),d=i-(c=z),f=n-(u=C),r.zoomAnimCVBTarget={x:t,y:o,w:i,h:n}}else{var B=w(e/a);s=l+p*B,E=m+h*B,z=c+d*B,C=u+f*B}"in"===g&&(z>r.currentViewBox.w||z<i)||"out"===g&&(z<r.currentViewBox.w||z>i)||r.setViewBox(s,E,z,C),r.zoomAnimID=r.requestAnimationFrame(y)}else r.zoomAnimStartTime=null,r.zoomAnimCVBTarget=null,r.currentViewBox.w!==i&&r.setViewBox(t,o,i,n),r.$map.trigger("afterZoom",{x1:t,y1:o,x2:t+i,y2:o+n})};y()},requestAnimationFrame:function(t){return this._requestAnimationFrameFn.call(window,t)},cancelAnimationFrame:function(t){this._cancelAnimationFrameFn.call(window,t)},_requestAnimationFrameFn:(n=(new Date).getTime(),a=function(t){var e=(new Date).getTime();if(!(e-n>16))return setTimeout(function(){a(t)},0);n=e,t(e)},window.requestAnimationFrame||window.webkitRequestAnimationFrame||window.mozRequestAnimationFrame||window.msRequestAnimationFrame||window.oRequestAnimationFrame||a),_cancelAnimationFrameFn:window.cancelAnimationFrame||window.webkitCancelAnimationFrame||window.webkitCancelRequestAnimationFrame||window.mozCancelAnimationFrame||window.mozCancelRequestAnimationFrame||window.msCancelAnimationFrame||window.msCancelRequestAnimationFrame||window.oCancelAnimationFrame||window.oCancelRequestAnimationFrame||clearTimeout,setViewBox:function(t,e,o,i){this.currentViewBox.x=t,this.currentViewBox.y=e,this.currentViewBox.w=o,this.currentViewBox.h=i,this.paper.setViewBox(t,e,o,i,!1)},_nonAnimatedAttrs:["arrow-end","arrow-start","gradient","class","cursor","text-anchor","font","font-family","font-style","font-weight","letter-spacing","src","href","target","title","stroke-dasharray","stroke-linecap","stroke-linejoin","stroke-miterlimit"],animate:function(t,e,o,n){if(t)if(o>0){for(var a={},s=0;s<this._nonAnimatedAttrs.length;s++){var r=this._nonAnimatedAttrs[s];e[r]!==i&&(a[r]=e[r])}t.attr(a),t.animate(e,o,"linear",function(){n&&n()})}else t.attr(e),n&&n()},isRaphaelBBoxBugPresent:function(){var t=this.paper.text(-50,-50,"TEST"),e=t.getBBox();return t.remove(),0===e.width&&0===e.height},defaultOptions:{map:{cssClass:"map",tooltip:{cssClass:"mapTooltip"},defaultArea:{attrs:{fill:"#343434",stroke:"#5d5d5d","stroke-width":1,"stroke-linejoin":"round"},attrsHover:{fill:"#f38a03",animDuration:300},text:{position:"inner",margin:10,attrs:{"font-size":15,fill:"#c7c7c7"},attrsHover:{fill:"#eaeaea",animDuration:300}},target:"_self",cssClass:"area"},defaultPlot:{type:"circle",size:15,attrs:{fill:"#0088db",stroke:"#fff","stroke-width":0,"stroke-linejoin":"round"},attrsHover:{"stroke-width":3,animDuration:300},text:{position:"right",margin:10,attrs:{"font-size":15,fill:"#c7c7c7"},attrsHover:{fill:"#eaeaea",animDuration:300}},target:"_self",cssClass:"plot"},defaultLink:{factor:.5,attrs:{stroke:"#0088db","stroke-width":2},attrsHover:{animDuration:300},text:{position:"inner",margin:10,attrs:{"font-size":15,fill:"#c7c7c7"},attrsHover:{fill:"#eaeaea",animDuration:300}},target:"_self",cssClass:"link"},zoom:{enabled:!1,minLevel:0,maxLevel:10,step:.25,mousewheel:!0,touch:!0,animDuration:200,animEasing:"linear",buttons:{reset:{cssClass:"zoomButton zoomReset",content:"&#8226;",title:"Reset zoom"},in:{cssClass:"zoomButton zoomIn",content:"+",title:"Zoom in"},out:{cssClass:"zoomButton zoomOut",content:"&#8722;",title:"Zoom out"}}}},legend:{redrawOnResize:!0,area:[],plot:[]},areas:{},plots:{},links:{}},legendDefaultOptions:{area:{cssClass:"areaLegend",display:!0,marginLeft:10,marginLeftTitle:5,marginBottomTitle:10,marginLeftLabel:10,marginBottom:10,titleAttrs:{"font-size":16,fill:"#343434","text-anchor":"start"},labelAttrs:{"font-size":12,fill:"#343434","text-anchor":"start"},labelAttrsHover:{fill:"#787878",animDuration:300},hideElemsOnClick:{enabled:!0,opacity:.2,animDuration:300},slices:[],mode:"vertical"},plot:{cssClass:"plotLegend",display:!0,marginLeft:10,marginLeftTitle:5,marginBottomTitle:10,marginLeftLabel:10,marginBottom:10,titleAttrs:{"font-size":16,fill:"#343434","text-anchor":"start"},labelAttrs:{"font-size":12,fill:"#343434","text-anchor":"start"},labelAttrsHover:{fill:"#787878",animDuration:300},hideElemsOnClick:{enabled:!0,opacity:.2,animDuration:300},slices:[],mode:"vertical"}}},s.version="2.2.0",t.mapael===i&&(t.mapael=s),t.fn.mapael=function(e){return this.each(function(){t.data(this,"mapael")&&t.data(this,"mapael").destroy(),t.data(this,"mapael",new s(this,e))})},s}(o(1),o(255),o(254))},254:function(t,e,o){var i,n,a;
/*!
 * jQuery Mousewheel 3.1.13
 *
 * Copyright jQuery Foundation and other contributors
 * Released under the MIT license
 * http://jquery.org/license
 */n=[o(1)],void 0===(a="function"==typeof(i=function(t){var e,o,i=["wheel","mousewheel","DOMMouseScroll","MozMousePixelScroll"],n="onwheel"in document||document.documentMode>=9?["wheel"]:["mousewheel","DomMouseScroll","MozMousePixelScroll"],a=Array.prototype.slice;if(t.event.fixHooks)for(var s=i.length;s;)t.event.fixHooks[i[--s]]=t.event.mouseHooks;var r=t.event.special.mousewheel={version:"3.1.12",setup:function(){if(this.addEventListener)for(var e=n.length;e;)this.addEventListener(n[--e],l,!1);else this.onmousewheel=l;t.data(this,"mousewheel-line-height",r.getLineHeight(this)),t.data(this,"mousewheel-page-height",r.getPageHeight(this))},teardown:function(){if(this.removeEventListener)for(var e=n.length;e;)this.removeEventListener(n[--e],l,!1);else this.onmousewheel=null;t.removeData(this,"mousewheel-line-height"),t.removeData(this,"mousewheel-page-height")},getLineHeight:function(e){var o=t(e),i=o["offsetParent"in t.fn?"offsetParent":"parent"]();return i.length||(i=t("body")),parseInt(i.css("fontSize"),10)||parseInt(o.css("fontSize"),10)||16},getPageHeight:function(e){return t(e).height()},settings:{adjustOldDeltas:!0,normalizeOffset:!0}};function l(i){var n=i||window.event,s=a.call(arguments,1),l=0,h=0,c=0,d=0,u=0,f=0;if((i=t.event.fix(n)).type="mousewheel","detail"in n&&(c=-1*n.detail),"wheelDelta"in n&&(c=n.wheelDelta),"wheelDeltaY"in n&&(c=n.wheelDeltaY),"wheelDeltaX"in n&&(h=-1*n.wheelDeltaX),"axis"in n&&n.axis===n.HORIZONTAL_AXIS&&(h=-1*c,c=0),l=0===c?h:c,"deltaY"in n&&(c=-1*n.deltaY,l=c),"deltaX"in n&&(h=n.deltaX,0===c&&(l=-1*h)),0!==c||0!==h){if(1===n.deltaMode){var g=t.data(this,"mousewheel-line-height");l*=g,c*=g,h*=g}else if(2===n.deltaMode){var w=t.data(this,"mousewheel-page-height");l*=w,c*=w,h*=w}if(d=Math.max(Math.abs(c),Math.abs(h)),(!o||d<o)&&(o=d,m(n,d)&&(o/=40)),m(n,d)&&(l/=40,h/=40,c/=40),l=Math[l>=1?"floor":"ceil"](l/o),h=Math[h>=1?"floor":"ceil"](h/o),c=Math[c>=1?"floor":"ceil"](c/o),r.settings.normalizeOffset&&this.getBoundingClientRect){var x=this.getBoundingClientRect();u=i.clientX-x.left,f=i.clientY-x.top}return i.deltaX=h,i.deltaY=c,i.deltaFactor=o,i.offsetX=u,i.offsetY=f,i.deltaMode=0,s.unshift(i,l,h,c),e&&clearTimeout(e),e=setTimeout(p,200),(t.event.dispatch||t.event.handle).apply(this,s)}}function p(){o=null}function m(t,e){return r.settings.adjustOldDeltas&&"mousewheel"===t.type&&e%120==0}t.fn.extend({mousewheel:function(t){return t?this.bind("mousewheel",t):this.trigger("mousewheel")},unmousewheel:function(t){return this.unbind("mousewheel",t)}})})?i.apply(e,n):i)||(t.exports=a)},255:function(t,e){t.exports=window.Raphael},256:function(t,e,o){"use strict";o.r(e);var i=o(159);o.n(i),o.d(e,"Mapael",function(){return i})}});if("object"==typeof o){var i=["object"==typeof module&&"object"==typeof module.exports?module.exports:null,"undefined"!=typeof window?window:null,t&&t!==window?t:null];for(var n in o)i[0]&&(i[0][n]=o[n]),i[1]&&"__esModule"!==n&&(i[1][n]=o[n]),i[2]&&(i[2][n]=o[n])}}(this);