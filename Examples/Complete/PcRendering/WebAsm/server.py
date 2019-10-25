import sys
import http.server
import socketserver
import threading
import webbrowser

PORT = 8000
DEBUG_PORT = 8080

Handler = http.server.SimpleHTTPRequestHandler
Handler.extensions_map['.wasm'] = 'application/wasm'

DebugHandler = http.server.SimpleHTTPRequestHandler
DebugHandler.extensions_map['.pdb'] = 'application/pdb'

with socketserver.TCPServer(("", PORT), Handler) as httpd:
    print("python 3 serving at port", PORT)    
    httpd.serve_forever()


#with socketserver.TCPServer(("", DEBUG_PORT), DebugHandler) as httpdD:
#    print("python 3 serving debug pdbs at port", DEBUG_PORT)
#    httpdDThr = threading.Thread(target=httpdD.serve_forever)
#    httpdDThr.daemon = True
#    httpdDThr.start()       

