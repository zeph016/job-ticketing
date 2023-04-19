function ScrollToBottomComment (id) {
  const element = document.getElementById(id); 
  element.scrollTop = element.scrollHeight;
}


function CheckScrollbar(id) {
  var div = document.getElementById(id);
  var hasHorizontalScrollbar = div.scrollWidth > div.clientWidth;
  var hasVerticalScrollbar = div.scrollHeight > div.clientHeight;
  if (hasHorizontalScrollbar || hasVerticalScrollbar)
    return true;
  else
    return false;
}