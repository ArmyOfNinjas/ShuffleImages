# ShuffleImages
Image Detection and sorting


This API is used to recognize objects in array of images, re-order them in a visually appropirate way, and store marked images in a Firebase Storage.


The service receives a json file as a parameter that includes an array of Image Names, and a user information. 
Then it uses ML.NET library to detect objects in the images and saves them in a cloud storage.
After that it takes object position in images and compares it to objects positions on other images to find if objects may be colliding in neighbour images.
After that the sorting is done, and an array of image names is reterned back to the user.
