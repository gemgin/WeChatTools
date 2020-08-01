# -*- coding: utf-8 -*-

import sys, os, struct, zlib

if __name__ == "__main__":
    if len(sys.argv) < 3:
        print 'few argument'
    else:
        pkgfilename = sys.argv[1]
        outdirname = sys.argv[2]
        pkgfile = file(pkgfilename, 'rb')
        pkgfile.read(4)
        filenums, = struct.unpack('I', pkgfile.read(4))
        filename_table_offset, = struct.unpack('I', pkgfile.read(4))
        filename_table_len, = struct.unpack('I', pkgfile.read(4))
        pkgfile.seek(filename_table_offset)
        for index in range(filenums):
            name_len, = struct.unpack('H', pkgfile.read(2))
            name = pkgfile.read(name_len)
            pkgfile.read(4)
            offset, = struct.unpack('I', pkgfile.read(4))
            size, = struct.unpack('I', pkgfile.read(4))
            zlib_size, = struct.unpack('I', pkgfile.read(4))
            current_pos = pkgfile.tell()
            pkgfile.seek(offset)
            text = pkgfile.read(zlib_size)
            text = zlib.decompress(text)
            pkgfile.seek(current_pos)
            outfilename = os.path.join(outdirname, os.path.join(os.path.splitext(os.path.basename(pkgfilename))[0], name))
            print u'start [%d/%d]: ' %(index+1, filenums), os.path.join(os.path.splitext(os.path.basename(pkgfilename))[0], name)
            if not os.path.exists(os.path.dirname(outfilename)):
                os.makedirs(os.path.dirname(outfilename))
            file(outfilename, 'wb').write(text)