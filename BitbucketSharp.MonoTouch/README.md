# BitbucketSharp

Showin some love for Bitbucket

## Description

A C# implementation of Bitbucket's REST request framework.

## Example Usage

	//Instantiate a client with your username and password
	var client = BitbucketSharp.Client("username", "password");

	//Request information about a user
	var userInfo = client.Users["thedillonb"].GetInfo();

	//Request information about a specific repository
	var repoInfo = client.Users["thedillonb"].Repositories["bitbucketsharp"].GetInfo();

