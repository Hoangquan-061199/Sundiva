/*
  RoxyFileman - web based file manager. Ready to use with CKEditor, TinyMCE. 
  Can be easily integrated with any other WYSIWYG editor or CMS.

  Copyright (C) 2013, RoxyFileman.com - Lyubomir Arsov. All rights reserved.
  For licensing, see LICENSE.txt or http://RoxyFileman.com/license

  This program is free software: you can redistribute it and/or modify
  it under the terms of the GNU General Public License as published by
  the Free Software Foundation, either version 3 of the License.

  This program is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
  GNU General Public License for more details.

  You should have received a copy of the GNU General Public License
  along with this program.  If not, see <http://www.gnu.org/licenses/>.

  Contact: Lyubomir Arsov, liubo (at) web-lobby.com
*/
function FileSelected(file) {
  /**
   * file is an object containing following properties:
   * 
   * fullPath - path to the file - absolute from your site root
   * path - directory in which the file is located - absolute from your site root
   * size - size of the file in bytes
   * time - timestamo of last modification
   * name - file name
   * ext - file extension
   * width - if the file is image, this will be the width of the original image, 0 otherwise
   * height - if the file is image, this will be the height of the original image, 0 otherwise
   * 
   */
    var view = RoxyUtils.GetUrlParam('view');
    var name = RoxyUtils.GetUrlParam('name');
    var type = RoxyUtils.GetUrlParam('typeview');
    var integration = RoxyUtils.GetUrlParam('integration');
    switch (integration) {
        case "folderadmin": {
            var win = (window.opener ? window.opener : window.parent);
            if (type == 1) {
                win.AlbumTinyMce(file, view, name)
            }
            else if (type == 2) {
                win.FileTinyMce(file, view, name)
            }
            else if (type == 3) {
                win.ImageTinyMce(file, view, name)
            }
            else if (type == 4) {
                win.MultiFileTinyMce(file, view, name)
            }
            else if (type == 5) {
                win.ColorTableTinyMce(file, view, name)
            }
            else if (type == 6) {
                win.AlbumModuleProductTinyMce(file, view, name);
            }
            else if (type == 7) {
                win.SlideDetailTinyMce(file, view, name);
            }
            else {
                win.UpdatePictureTinyMce(file, view, name)
            }
            self.close();
            return;
        }
        default:
            alert('"' + file.fullPath + "\" selected.\n To integrate with CKEditor or TinyMCE change INTEGRATION setting in conf.json. For more details see the Installation instructions at http://www.roxyfileman.com/install.");
    }
}
