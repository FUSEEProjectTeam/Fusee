## Folder Contents

This folder contains the source data to build FUSEE's website (hosted at http://fusee3d.org).
The content's raw format is created as Markdown (md) files and then processed by
the [Hugo](http://gohugo.io/) static content generator into HTML.

The editable contents is beneath the `content` folder.

An adaption of the 
[Beautiful Hugo Theme](https://themes.gohugo.io/theme/beautifulhugo/)
 is used to generate the FUSEE website content. The adapted theme
is below the `themes` folder. 

All folders found here are Hugo standard folders. Read the Hugo docs to understand
their purpose and how they contribute in the course of generating the FUSEE website.

## How To Develop FUSEE Website Content

Make sure you have a current version of Hugo in your system and you can access it by
typing the `hugo` command from the shell. The editor used here to create md files is
Visual Studio Code, so you might want to install this, too.

Double-clicking on `DevelopMe.cmd` will do three things:

1. Open up Visual Studio code at the root of this folder
2. Start Hugo in "Server" Mode to act as a local web server on port 1313 serving
   the rendered content directly from memory for preview purposes.
3. Start the default browser on `http://localhost:1313` to preview all changes.
   Hugo keeps track of any changes and re-renders the contents immediately.

Edit, add, or remove `*.md` files below the `contents` folder. Change the contents 
of `config.toml` to edit menu entries and global appearance settings.
All changes are immediately reflected in the browser.

## How to build content

To statically generate HTML files from the content, double-click on `BuildMe.cmd` this will
call the `hugo` command to render the markdown contents to HTML/CSS/JS files.

Currently the combination of Hugo and the used theme generates absolute URL references 
in the generated HTML output. Thus it is necessary to specify the target root URL where
the generated content is intended to be served from. Open `BuildMe.cmd` and see the hugo
command line to see how the target root URL is specified there.

The generated content is placed under %FuseeRoot%/docs. This is a special folder
recognized by GitHub as the source for GitHub pages content. Note that the content 
generation process here uses Hugo and NOT Jeckyll - the site generator preferred by
GitHub and integrated into GitHub-Pages content generation. This choice was made
due to possible nightmares you might encounter when trying to install Jeckyll on a standard 
Windows machine.


## Creating Screen Casts

The Screen Casts are created using ScreenToGif. The resulting casts should not contain 
of more than 5 steps and should not be larger than 4MB. Consider splitting the
cast in several steps.

Every step should be explained in a numbered list before the cast. 

Size is 1024 x 600 at 96dpi (scale to these settings in ScreenToGif). 

Apply a progress bar in Greenery (rgb(136, 176, 75))

Steps should be numbered at the right top region of the cast image. Step numbers
are inserted in ScreenToGif as "Free Text", Font: Humnst777, Size 60 (120 on 2xHiDPI),

