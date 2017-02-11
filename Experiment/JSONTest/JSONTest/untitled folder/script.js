requirejs.config({
  paths: { ace: ['https://cdnjs.cloudflare.com/ajax/libs/ace/1.2.6/ace.js'] }
})

$('h1').text("loading ace...");
requirejs([ 'ace/ace'], function(ace) {
  $('h1').text("ace loaded.")
  console.log(ace)
  ace.config.set("packaged", true)
  ace.config.set("basePath", require.toUrl("ace"))
  editor = ace.edit('editor')
  editor.setTheme("ace/theme/monokai")
  return
})
